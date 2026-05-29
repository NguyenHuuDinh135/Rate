using backend.Application.Users.Queries.GetMyUserInfo;
using backend.Application.Common.Models;
using backend.Infrastructure.Data;
using backend.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Web.Endpoints;

public class UserEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/users";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetMe).RequireAuthorization();
        group.MapGet("/id/{id}", GetById).RequireAuthorization();
        group.MapGet("/all", GetAll).RequireAuthorization();
        group.MapPost("/create", Create).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/delete/{id}", Delete).RequireAuthorization();
        group.MapGet("/activity", GetActivity).RequireAuthorization();
        group.MapPut("/me", UpdateMe).RequireAuthorization();
    }

    public static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> GetMe(ISender sender)
    {
        var result = await sender.Send(new GetMyUserInfoQuery());
        return result is null ? TypedResults.Unauthorized() : TypedResults.Ok(result);
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<Result>>> UpdateMe(
        UserManager<ApplicationUser> userManager,
        HttpContext httpContext,
        UpdateMeRequest request)
    {
        var userId = httpContext.User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                     ?? httpContext.User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userId))
            return TypedResults.NotFound();

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return TypedResults.NotFound();

        user.UserName = request.UserName.Trim();
        user.Email = request.Email.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return TypedResults.BadRequest(Result.Failure(updateResult.Errors.Select(x => x.Description)));
        }

        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<AdminUserDto>, NotFound>> GetById(
        UserManager<ApplicationUser> userManager,
        string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var roles = await userManager.GetRolesAsync(user);
        return TypedResults.Ok(ToAdminUserDto(user, roles));
    }

    public static async Task<IReadOnlyList<AdminUserDto>> GetAll(UserManager<ApplicationUser> userManager)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var result = new List<AdminUserDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(ToAdminUserDto(user, roles));
        }

        return result;
    }

    public static async Task<Results<Ok<Result<string>>, BadRequest<Result<string>>>> Create(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        CreateAdminUserRequest request)
    {
        var requestedRoles = await ValidateRoles(roleManager, request.Roles);
        if (requestedRoles is null)
        {
            return TypedResults.BadRequest(Result<string>.Failure(["One or more selected roles do not exist."]));
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName.Trim(),
            Email = request.Email.Trim(),
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return TypedResults.BadRequest(Result<string>.Failure(createResult.Errors.Select(x => x.Description)));
        }

        if (requestedRoles.Count > 0)
        {
            var roleResult = await userManager.AddToRolesAsync(user, requestedRoles);
            if (!roleResult.Succeeded)
            {
                return TypedResults.BadRequest(Result<string>.Failure(roleResult.Errors.Select(x => x.Description)));
            }
        }

        return TypedResults.Ok(Result<string>.Success(user.Id));
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<Result>>> Update(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        UpdateAdminUserRequest request)
    {
        var user = await userManager.FindByIdAsync(request.Id);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var requestedRoles = await ValidateRoles(roleManager, request.Roles);
        if (requestedRoles is null)
        {
            return TypedResults.BadRequest(Result.Failure(["One or more selected roles do not exist."]));
        }

        user.UserName = request.UserName.Trim();
        user.Email = request.Email.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return TypedResults.BadRequest(Result.Failure(updateResult.Errors.Select(x => x.Description)));
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return TypedResults.BadRequest(Result.Failure(removeResult.Errors.Select(x => x.Description)));
        }

        if (requestedRoles.Count > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, requestedRoles);
            if (!addResult.Succeeded)
            {
                return TypedResults.BadRequest(Result.Failure(addResult.Errors.Select(x => x.Description)));
            }
        }

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<Result>>> Delete(
        UserManager<ApplicationUser> userManager,
        string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? TypedResults.NoContent()
            : TypedResults.BadRequest(Result.Failure(result.Errors.Select(x => x.Description)));
    }

    public static async Task<IReadOnlyList<UserActivityDto>> GetActivity(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        var userNames = await userManager.Users
            .AsNoTracking()
            .Select(x => new { x.Id, Name = x.UserName ?? x.Email ?? x.Id })
            .ToDictionaryAsync(x => x.Id, x => x.Name);

        var userActivities = await userManager.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(20)
            .Select(x => new ActivityProjection(
                x.UserName ?? x.Email ?? x.Id,
                "Tạo tài khoản người dùng",
                "Người dùng",
                x.CreatedAt))
            .ToListAsync();

        var bookings = await db.Bookings
            .AsNoTracking()
            .OrderByDescending(x => x.BookingDateTime)
            .Take(20)
            .Select(x => new { x.Id, x.UserId, x.SeatRow, x.SeatNumber, x.BookingDateTime })
            .ToListAsync();

        var bookingActivities = bookings
            .Select(x => new ActivityProjection(
                userNames.GetValueOrDefault(x.UserId, x.UserId),
                $"Đặt vé #{x.Id} ghế {x.SeatRow}{x.SeatNumber}",
                "Đặt vé",
                x.BookingDateTime));

        var payments = await db.Payments
            .AsNoTracking()
            .OrderByDescending(x => x.PaymentDateTime)
            .Take(20)
            .Select(x => new { x.Id, x.UserId, x.PaymentDateTime })
            .ToListAsync();

        var paymentActivities = payments
            .Select(x => new ActivityProjection(
                userNames.GetValueOrDefault(x.UserId, x.UserId),
                $"Thanh toán hóa đơn #{x.Id}",
                "Thanh toán",
                x.PaymentDateTime));

        return userActivities
            .Concat(bookingActivities)
            .Concat(paymentActivities)
            .OrderByDescending(x => x.OccurredAt)
            .Take(50)
            .Select((x, index) => new UserActivityDto(index + 1, x.UserName, x.Action, x.Type, x.OccurredAt))
            .ToList();
    }

    private static AdminUserDto ToAdminUserDto(ApplicationUser user, IEnumerable<string> roles)
        => new(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            roles.ToList(),
            user.CreatedAt,
            user.UpdatedAt);

    private static async Task<List<string>?> ValidateRoles(RoleManager<IdentityRole> roleManager, IEnumerable<string>? roles)
    {
        var requested = roles?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        foreach (var role in requested)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                return null;
            }
        }

        return requested;
    }

    public sealed record AdminUserDto(
        string Id,
        string UserName,
        string Email,
        bool EmailConfirmed,
        List<string> Roles,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public sealed record CreateAdminUserRequest(string UserName, string Email, string Password, List<string>? Roles);

    public sealed record UpdateAdminUserRequest(string Id, string UserName, string Email, List<string>? Roles);
    
    public sealed record UpdateMeRequest(string UserName, string Email);

    public sealed record UserActivityDto(int Id, string UserName, string Action, string Type, DateTime OccurredAt);

    private sealed record ActivityProjection(string UserName, string Action, string Type, DateTime OccurredAt);
}
