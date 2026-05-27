using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace WebUI.Shared.Services.Storage;

public class WebTokenStorage(ILocalStorageService localStorage) : ITokenStorage
{
    private const string AccessTokenKey = "rate.access_token";
    private const string RefreshTokenKey = "rate.refresh_token";

    public async Task SetAccessTokenAsync(string token)
    {
        try { await localStorage.SetItemAsStringAsync(AccessTokenKey, token); }
        catch (JSException) { /* SSR or JS not ready */ }
        catch (InvalidOperationException) { /* SSR or JS not ready */ }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try { return await localStorage.GetItemAsStringAsync(AccessTokenKey); }
        catch (JSException) { return null; }
        catch (InvalidOperationException) { return null; }
    }

    public async Task SetRefreshTokenAsync(string token)
    {
        try { await localStorage.SetItemAsStringAsync(RefreshTokenKey, token); }
        catch (JSException) { /* SSR or JS not ready */ }
        catch (InvalidOperationException) { /* SSR or JS not ready */ }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try { return await localStorage.GetItemAsStringAsync(RefreshTokenKey); }
        catch (JSException) { return null; }
        catch (InvalidOperationException) { return null; }
    }

    public async Task ClearAsync()
    {
        try
        {
            await localStorage.RemoveItemAsync(AccessTokenKey);
            await localStorage.RemoveItemAsync(RefreshTokenKey);
        }
        catch (JSException) { }
        catch (InvalidOperationException) { }
    }

    public Task SetTokenAsync(string token)
        => SetAccessTokenAsync(token);

    public Task<string?> GetTokenAsync()
        => GetAccessTokenAsync();

    public Task ClearTokenAsync()
        => ClearAsync();
}
