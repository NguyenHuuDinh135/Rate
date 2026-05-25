using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IPermissionApi
{
    [Get("/api/permissions/me")]
    Task<List<string>> GetMyPermissionsAsync();

    [Get("/api/permissions/all")]
    Task<List<string>> GetAllPermissionsAsync();

    [Get("/api/permissions/roles/all")]
    Task<List<RoleDto>> GetAllRolesAsync();

    [Get("/api/permissions/roles/{roleName}")]
    Task<List<string>> GetRolePermissionsAsync(string roleName);

    [Put("/api/permissions/roles/{roleName}")]
    Task UpdateRolePermissionsAsync(string roleName, [Body] PermissionUpdateRequest payload);

    [Post("/api/permissions/roles/create")]
    Task<OperationResultDto<string>> CreateRoleAsync([Body] CreateRoleRequest payload);

    [Delete("/api/permissions/roles/{roleName}")]
    Task DeleteRoleAsync(string roleName);
}

public record RoleDto(string Id, string Name, string NormalizedName, int PermissionsCount);

public record PermissionUpdateRequest(List<string> Permissions);

public record CreateRoleRequest(string Name);
