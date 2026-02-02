using Duende.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Identity.API.Configuration;
using Identity.API.Data;
using Identity.API.Models;


var builder = WebApplication.CreateBuilder(args);

// ==========================
// Add services
// ==========================

// MVC (IdentityServer UI cần Views)
builder.Services.AddControllersWithViews();

// ==========================
// Database
// ==========================

// 👉 ĐỔI UseNpgsql thành UseSqlServer nếu bạn dùng SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("IdentityDb")
    ));

// ==========================
// ASP.NET Identity
// ==========================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// =============
// Cors
// ===============

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                // .AllowAnyOrigin()    // hoặc WithOrigins(...)
                .WithOrigins("http://localhost:5173") // thay port fe
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// ==========================
// IdentityServer
// ==========================

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
})
.AddAspNetIdentity<ApplicationUser>()
.AddInMemoryIdentityResources(Config.GetResources())
.AddInMemoryApiScopes(Config.GetApiScopes())
.AddInMemoryApiResources(Config.GetApis())
.AddInMemoryClients(Config.GetClients(builder.Configuration))
.AddDeveloperSigningCredential(); // 👉 tự sinh tempkey.jwk (DEV)

builder.Services.AddEndpointsApiExplorer();
// ==========================
// Build app
// ==========================

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>()       // ✅ QUAN TRỌNG
    .AddEnvironmentVariables();


var app = builder.Build();

// ==========================
// HTTP pipeline
// ==========================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowFrontend");

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();   // 🔴 BẮT BUỘC
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
