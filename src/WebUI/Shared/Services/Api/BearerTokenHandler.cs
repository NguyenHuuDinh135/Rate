using System.Net;
using System.Net.Http.Headers;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WebUI.Shared.Services.Auth;
using WebUI.Shared.Services.Storage;
using WebUI.Shared.Store.Auth;

namespace WebUI.Shared.Services.Api;

public class BearerTokenHandler(
    ITokenStorage tokenStorage,
    IServiceProvider serviceProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenStorage.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token) && request.Headers.Authorization is null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            await tokenStorage.ClearAsync();

            var authState = serviceProvider.GetService<AuthStateService>();
            if (authState is not null)
            {
                authState.MarkSessionExpired();
            }
            else if (serviceProvider.GetService<AuthenticationStateProvider>() is CustomAuthenticationStateProvider provider)
            {
                provider.NotifyUserLogout();
            }

            serviceProvider.GetService<IDispatcher>()?
                .Dispatch(new AuthSessionExpiredAction("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại."));
        }

        return response;
    }
}
