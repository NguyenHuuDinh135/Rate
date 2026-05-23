using Refit;
using WebFrontend.Shared.Models.Auth;

namespace WebFrontend.Shared.Services.Api;

public interface IPermissionApi
{
    [Get("/api/permissions/me")]
    Task<List<string>> GetMyPermissionsAsync();

    [Get("/api/permissions/all")]
    Task<List<string>> GetAllPermissionsAsync();

    [Get("/api/permissions/roles/all")]
    Task<List<RoleDto>> GetAllRolesAsync();
}

public record RoleDto(string Id, string Name, string NormalizedName, int PermissionsCount);
