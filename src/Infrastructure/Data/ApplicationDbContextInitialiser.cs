using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;
using backend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using backend.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using backend.Application.Common.Contracts;
using Hangfire;
using backend.Application.Common.BackgroundJobs;

namespace backend.Infrastructure.Data
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var applyMigrations = configuration.GetValue("Database:ApplyMigrationsOnStartup", true);
            var resetOnStartup = configuration.GetValue<bool>("Database:ResetOnStartup");
            var seedOnStartup = configuration.GetValue("Database:SeedOnStartup", true);

            if (!applyMigrations)
            {
                return;
            }

            await initialiser.InitialiseAsync(resetOnStartup);
            if (seedOnStartup)
            {
                await initialiser.SeedAsync();
            }

            // Schedule AI Background Jobs
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            recurringJobManager.AddOrUpdate<IEmbeddingSyncJob>(
                "sync-movie-embeddings", 
                job => job.SyncMovieEmbeddingsAsync(), 
                Cron.Hourly);

            recurringJobManager.AddOrUpdate<IEmbeddingSyncJob>(
                "sync-review-embeddings", 
                job => job.SyncReviewEmbeddingsAsync(), 
                Cron.Hourly);
        }
    }

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _publishEndpoint = publishEndpoint;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public async Task InitialiseAsync(bool resetOnStartup = false)
        {
            try
            {
                _logger.LogInformation("Bắt đầu khởi tạo Database...");
                if (resetOnStartup)
                {
                    _logger.LogWarning("Database reset is enabled. Dropping and recreating schema.");
                    await _context.Database.EnsureDeletedAsync();
                    await _context.Database.EnsureCreatedAsync();
                }
                else
                {
                    await _context.Database.MigrateAsync();
                }
                _logger.LogInformation("Khởi tạo Database thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Bắt đầu Seed dữ liệu từ JSON files...");
                await TrySeedAsync();
                _logger.LogInformation("✅ Seed dữ liệu thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task<T[]> LoadSeedDataAsync<T>(string fileName)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", fileName);
            if (!File.Exists(path))
            {
                // Fallback for development (if BaseDirectory doesn't have it yet)
                path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Infrastructure", "Data", "SeedData", fileName);
            }

            if (!File.Exists(path))
            {
                _logger.LogWarning($"Seed data file not found: {fileName} at {path}");
                return Array.Empty<T>();
            }

            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<T[]>(json, _jsonOptions) ?? Array.Empty<T>();
        }

        public async Task TrySeedAsync()
        {
            // Seed Identity Roles
            var roles = new[] { "Admin", "ApiUser" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Identity Users from JSON
            var userData = await LoadSeedDataAsync<UserSeedDto>("Users.json");
            foreach (var userDto in userData)
            {
                if (await _userManager.FindByIdAsync(userDto.Id) == null)
                {
                    var user = new ApplicationUser
                    {
                        Id = userDto.Id,
                        UserName = userDto.Email,
                        Email = userDto.Email,
                        EmailConfirmed = true,
                        // Mapping other properties if needed
                    };

                    var result = await _userManager.CreateAsync(user, userDto.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, userDto.Role);
                    }
                }
            }

            // Seed Admin user mặc định if not exists
            if (!await _userManager.Users.AnyAsync(u => u.UserName == "admin@local"))
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@local",
                    Email = "admin@local",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed Genres
            if (!_context.Genres.Any())
            {
                var genres = await LoadSeedDataAsync<Genre>("Genres.json");
                _context.Genres.AddRange(genres);
                await _context.SaveChangesAsync();
            }

            // Seed Movies
            if (!_context.Movies.Any())
            {
                var movies = await LoadSeedDataAsync<Movie>("Movies.json");
                _context.Movies.AddRange(movies);
                await _context.SaveChangesAsync();
            }

            // Seed MovieGenres
            if (!_context.MovieGenres.Any())
            {
                var movieGenres = await LoadSeedDataAsync<MovieGenre>("MovieGenres.json");
                _context.MovieGenres.AddRange(movieGenres);
                await _context.SaveChangesAsync();
            }

            // Seed Persons
            if (!_context.Persons.Any())
            {
                var persons = await LoadSeedDataAsync<Person>("Persons.json");
                _context.Persons.AddRange(persons);
                await _context.SaveChangesAsync();
            }

            // Seed MoviePersons
            if (!_context.MoviePersons.Any())
            {
                var moviePersons = await LoadSeedDataAsync<MoviePerson>("MoviePersons.json");
                _context.MoviePersons.AddRange(moviePersons);
                await _context.SaveChangesAsync();
            }

            // Seed Theaters
            if (!_context.Theaters.Any())
            {
                var theaters = await LoadSeedDataAsync<Theater>("Theaters.json");
                _context.Theaters.AddRange(theaters);
                await _context.SaveChangesAsync();
            }

            // Seed TheaterSeats
            if (!_context.TheaterSeats.Any())
            {
                var theaterSeats = await LoadSeedDataAsync<TheaterSeat>("TheaterSeats.json");
                _context.TheaterSeats.AddRange(theaterSeats);
                await _context.SaveChangesAsync();
            }

            // Seed Bookings
            if (!_context.Bookings.Any())
            {
                var bookings = await LoadSeedDataAsync<Booking>("Bookings.json");
                // Batch add for large datasets
                foreach (var batch in bookings.Chunk(500))
                {
                    _context.Bookings.AddRange(batch);
                    await _context.SaveChangesAsync();
                }
            }

            // Seed Payments
            if (!_context.Payments.Any())
            {
                var payments = await LoadSeedDataAsync<Payment>("Payments.json");
                _context.Payments.AddRange(payments);
                await _context.SaveChangesAsync();
            }
        }

        private class UserSeedDto
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
