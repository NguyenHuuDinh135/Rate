using System.Security.Claims;
using backend.Web.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using backend.Infrastructure.Identity;

namespace backend.Web.Endpoints;

public class PermissionEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/permissions";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetMyPermissions).RequireAuthorization();
        group.MapGet("/all", GetAllPermissions).RequireAuthorization();
        group.MapGet("/roles/all", GetAllRoles).RequireAuthorization();
    }

    public static async Task<Results<Ok<List<string>>, UnauthorizedHttpResult>> GetMyPermissions(
        ClaimsPrincipal principal,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var userId = userManager.GetUserId(principal);
        if (string.IsNullOrEmpty(userId))
        {
            return TypedResults.Unauthorized();
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var permissions = new List<string>();

        foreach (var roleName in roles)
        {
            permissions.Add(roleName);

            var role = await roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                var claims = await roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    if (claim.Type == "permission" && !string.IsNullOrEmpty(claim.Value))
                    {
                        permissions.Add(claim.Value);
                    }
                }
            }
        }

        return TypedResults.Ok(permissions.Distinct().ToList());
    }

    public static Task<Ok<List<string>>> GetAllPermissions()
    {
        var allPermissions = new List<string> { "Admin", "User" };
        return Task.FromResult(TypedResults.Ok(allPermissions));
    }

    public static async Task<Ok<List<RoleDto>>> GetAllRoles(RoleManager<IdentityRole> roleManager)
    {
        var roles = await roleManager.Roles.AsNoTracking().ToListAsync();
        var result = roles.Select(r => new RoleDto(
            r.Id,
            r.Name ?? string.Empty,
            r.NormalizedName ?? string.Empty,
            0
        )).ToList();

        return TypedResults.Ok(result);
    }
}

public record RoleDto(string Id, string Name, string NormalizedName, int PermissionsCount);
