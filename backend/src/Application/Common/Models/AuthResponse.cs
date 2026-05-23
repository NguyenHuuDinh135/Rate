using backend.Application.Common.Interfaces;

namespace backend.Application.Common.Models;

public class AuthResponse
{
    public UserDto User { get; set; } = null!;
    public AuthTokens Tokens { get; set; } = null!;
}

public class AuthTokens
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
