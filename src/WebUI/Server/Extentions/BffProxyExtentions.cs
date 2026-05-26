using System.Net.Http.Headers;
using System.Text.Json;

namespace WebUI.Server.Extensions;

public static class BffProxyExtensions
{
    public static IEndpointRouteBuilder MapBffProxy(this IEndpointRouteBuilder app)
    {
        app.Map("/api/{**slug}", async (HttpContext httpContext, IHttpClientFactory httpClientFactory) =>
        {
            var apiBaseUrl = GetApiBaseUrl(httpContext);

            var client = httpClientFactory.CreateClient("BFFProxy");

            var fullPath = httpContext.Request.Path.Value ?? string.Empty;
            var path = fullPath.StartsWith("/api", StringComparison.OrdinalIgnoreCase) 
                ? fullPath.Substring(4) 
                : fullPath;

            var query = httpContext.Request.QueryString.Value ?? string.Empty;
            var targetUrl = $"{apiBaseUrl}/api{path}{query}";

            using var requestMessage = new HttpRequestMessage(
                new HttpMethod(httpContext.Request.Method),
                targetUrl);

            CopyRequestHeaders(httpContext, requestMessage);
            AttachBearerTokenFromCookie(httpContext, requestMessage);
            AttachRequestBody(httpContext, requestMessage);

            using var responseMessage = await client.SendAsync(
                requestMessage,
                HttpCompletionOption.ResponseHeadersRead,
                httpContext.RequestAborted);

            await CopyProxyResponse(httpContext, responseMessage, path);
        });

        return app;
    }

    private static string GetApiBaseUrl(HttpContext context)
    {
        var apiBaseUrl = context.RequestServices
            .GetRequiredService<IConfiguration>()["ApiBaseUrl"] ?? "http://localhost:15000";

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT")) ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")))
        {
            apiBaseUrl = "http://webapi";
        }

        return apiBaseUrl;
    }

    private static void CopyRequestHeaders(HttpContext context, HttpRequestMessage request)
    {
        foreach (var header in context.Request.Headers)
        {
            if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                continue;

            if (header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                continue;

            request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    private static void AttachBearerTokenFromCookie(HttpContext context, HttpRequestMessage request)
    {
        if (request.Headers.Contains("Authorization"))
            return;

        if (context.Request.Cookies.TryGetValue("access_token", out var accessToken) &&
            !string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    private static void AttachRequestBody(HttpContext context, HttpRequestMessage request)
    {
        if (context.Request.ContentLength <= 0 &&
            !context.Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            return;
        }

        request.Content = new StreamContent(context.Request.Body);

        if (!string.IsNullOrWhiteSpace(context.Request.ContentType))
        {
            request.Content.Headers.ContentType =
                MediaTypeHeaderValue.Parse(context.Request.ContentType);
        }
    }

    private static async Task CopyProxyResponse(
        HttpContext context,
        HttpResponseMessage response,
        string path)
    {
        context.Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        context.Response.Headers.Remove("transfer-encoding");

        var isAuthRoute = path.StartsWith("/auth/", StringComparison.OrdinalIgnoreCase);
        var responseBodyString = string.Empty;

        if (isAuthRoute && response.IsSuccessStatusCode)
        {
            responseBodyString = await response.Content.ReadAsStringAsync(context.RequestAborted);
            TrySetAuthCookies(context, responseBodyString);
        }

        if (path.Equals("/auth/logout", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Cookies.Delete("access_token");
            context.Response.Cookies.Delete("refresh_token");
        }

        if (!string.IsNullOrEmpty(responseBodyString))
        {
            await context.Response.WriteAsync(responseBodyString, context.RequestAborted);
            return;
        }

        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }

    private static void TrySetAuthCookies(HttpContext context, string responseBodyString)
    {
        try
        {
            var tokenResult = JsonSerializer.Deserialize<WebUI.Shared.Models.Auth.TokenResponseDto>(
                responseBodyString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenResult is null || string.IsNullOrWhiteSpace(tokenResult.AccessToken))
                return;

            var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var isDev = env.IsDevelopment();

            context.Response.Cookies.Append("access_token", tokenResult.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(2),
                Path = "/"
            });

            if (!string.IsNullOrWhiteSpace(tokenResult.RefreshToken))
            {
                context.Response.Cookies.Append("refresh_token", tokenResult.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !isDev,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    Path = "/"
                });
            }
        }
        catch
        {
            // Ignore invalid auth response body
        }
    }
}