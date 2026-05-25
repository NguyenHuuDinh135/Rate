namespace WebUI.Shared.Models.Auth;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string UserName, string Email, string Password);
public record VerifyOtpRequest(string Email, string OtpCode);

public record TokenResponseDto(
    string? AccessToken,
    string? Username,
    string? Email,
    int? ExpiresIn,
    List<string>? Permissions);

public record UserProfileDto(string Id, string UserName, string Email, List<string> Permissions);

public record AdminUserDto(
    string Id,
    string UserName,
    string Email,
    bool EmailConfirmed,
    List<string> Roles,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateAdminUserRequest(string UserName, string Email, string Password, List<string> Roles);

public record UpdateAdminUserRequest(string Id, string UserName, string Email, List<string> Roles);

public record UserActivityDto(int Id, string UserName, string Action, string Type, DateTime OccurredAt);
