using backend.Application.Common.Models;

namespace backend.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<IReadOnlyList<UserDto>> GetUsersAsync();

    Task<Result> UpdateUserAsync(string userId, string fullName, string email);
}

public record UserDto(string Id, string Name, string Email, string? Role = null);
