namespace backend.Application.Common.Models;

public sealed class AuthTokenResult
{
    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public string UserId { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public IReadOnlyList<string> Roles { get; init; } = new List<string>();
}

