using Refit;
using WebUI.Shared.Models.Auth;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IUserApi
{
    [Get("/api/users/me")]
    Task<AuthUserDto> GetMeAsync();

    [Get("/api/users/all")]
    Task<List<AdminUserDto>> GetAllAsync();

    [Get("/api/users/id/{id}")]
    Task<AdminUserDto> GetByIdAsync(string id);

    [Post("/api/users/create")]
    Task<OperationResultDto<string>> CreateAsync([Body] CreateAdminUserRequest payload);

    [Put("/api/users/update")]
    Task UpdateAsync([Body] UpdateAdminUserRequest payload);

    [Delete("/api/users/delete/{id}")]
    Task DeleteAsync(string id);

    [Get("/api/users/activity")]
    Task<List<UserActivityDto>> GetActivityAsync();
}
