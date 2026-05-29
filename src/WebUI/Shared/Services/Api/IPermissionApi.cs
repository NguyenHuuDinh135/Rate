using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IPermissionApi
{
    [Get("/api/permissions/me")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<string>>> GetMyPermissionsAsync();

    [Get("/api/permissions/all")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<string>>> GetAllPermissionsAsync();

    [Get("/api/permissions/roles/all")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<RoleDto>>> GetAllRolesAsync();

    [Get("/api/permissions/roles/{roleName}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<string>>> GetRolePermissionsAsync(string roleName);

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
