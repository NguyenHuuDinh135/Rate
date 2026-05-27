namespace WebUI.Shared.Services.Storage;

public interface ITokenStorage
{
    Task SetAccessTokenAsync(string token);
    Task<string?> GetAccessTokenAsync();
    Task SetRefreshTokenAsync(string token);
    Task<string?> GetRefreshTokenAsync();
    Task ClearAsync();

    Task SetTokenAsync(string token);
    Task<string?> GetTokenAsync();
    Task ClearTokenAsync();
}
