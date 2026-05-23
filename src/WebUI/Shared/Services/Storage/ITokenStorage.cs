namespace WebUI.Shared.Services.Storage;

public interface ITokenStorage
{
    Task SetTokenAsync(string token);
    Task<string?> GetTokenAsync();
    Task ClearTokenAsync();
}
