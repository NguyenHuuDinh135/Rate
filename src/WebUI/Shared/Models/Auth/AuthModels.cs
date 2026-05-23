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
