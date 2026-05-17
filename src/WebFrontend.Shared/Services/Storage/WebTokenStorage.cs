using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace WebFrontend.Shared.Services.Storage;

public class WebTokenStorage(ILocalStorageService localStorage) : ITokenStorage
{
    private const string TokenKey = "access_token";

    public async Task SetTokenAsync(string token) 
    {
        try { await localStorage.SetItemAsStringAsync(TokenKey, token); }
        catch (JSException) { /* SSR or JS not ready */ }
        catch (InvalidOperationException) { /* SSR or JS not ready */ }
    }

    public async Task<string?> GetTokenAsync() 
    {
        try { return await localStorage.GetItemAsStringAsync(TokenKey); }
        catch (JSException) { return null; }
        catch (InvalidOperationException) { return null; }
    }

    public async Task ClearTokenAsync() 
    {
        try { await localStorage.RemoveItemAsync(TokenKey); }
        catch (JSException) { }
        catch (InvalidOperationException) { }
    }
}
