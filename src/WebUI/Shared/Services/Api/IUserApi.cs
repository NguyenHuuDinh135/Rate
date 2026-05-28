using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IUserApi
{
    [Get("/api/users/me")]
    Task<AuthUserDto> GetMeAsync();

    [Put("/api/users/me")]
    Task<OperationResultDto> UpdateMeAsync([Body] UpdateMeRequest payload);

    [Get("/api/users/all")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<AdminUserDto>>> GetAllAsync();

    [Get("/api/users/id/{id}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<AdminUserDto>> GetByIdAsync(string id);

    [Post("/api/users/create")]
    Task<OperationResultDto<string>> CreateAsync([Body] CreateAdminUserRequest payload);

    [Put("/api/users/update")]
    Task UpdateAsync([Body] UpdateAdminUserRequest payload);

    [Delete("/api/users/delete/{id}")]
    Task DeleteAsync(string id);

    [Get("/api/users/activity")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<UserActivityDto>>> GetActivityAsync();
}
