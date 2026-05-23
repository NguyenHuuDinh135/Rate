using WebFrontend.Shared.Services.Storage;

namespace MobileApp.Services;

public class MobileTokenStorage : ITokenStorage
{
    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.Default.GetAsync("auth_token");
    }

    public async Task RemoveTokenAsync()
    {
        SecureStorage.Default.Remove("auth_token");
        await Task.CompletedTask;
    }

    public async Task SetTokenAsync(string token)
    {
        await SecureStorage.Default.SetAsync("auth_token", token);
    }
}
