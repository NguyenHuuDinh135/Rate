using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace WebUI.Server.Extentions;

public static class BffProxyExtentions
{
    public static void MapBffProxy(this WebApplication app)
    {
        app.Map("/api/{**proxyPath}", async (string proxyPath, HttpContext context, IHttpClientFactory httpClientFactory) =>
        {
            try
            {
                var client = httpClientFactory.CreateClient("BFFProxy");
                
                // Dynamically resolve target ApiBaseUrl based on environment
                var apiBaseUrl = context.RequestServices.GetRequiredService<IConfiguration>()["ApiBaseUrl"] ?? "http://localhost:15000";
                
                if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT")) ||
                    !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")))
                {
                    apiBaseUrl = "http://webapi";
                }
                
                // Build target URL
                var relativePath = $"/api/{proxyPath}";
                var query = context.Request.QueryString.Value;
                var targetUrl = $"{apiBaseUrl.TrimEnd('/')}{relativePath}{query}";
                
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = new HttpMethod(context.Request.Method);
                requestMessage.RequestUri = new System.Uri(targetUrl, System.UriKind.Absolute);
                
                // Filter request headers to avoid conflicts
                var requestHeadersToSkip = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
                {
                    "Host",
                    "Content-Length",
                    "Connection"
                };

                // Copy request headers (especially Authorization)
                foreach (var header in context.Request.Headers)
                {
                    if (requestHeadersToSkip.Contains(header.Key))
                    {
                        continue;
                    }

                    if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                    {
                        requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                    }
                }
                
                // If request has a body, copy it
                if (context.Request.ContentLength > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
                {
                    var streamContent = new StreamContent(context.Request.Body);
                    requestMessage.Content = streamContent;
                    
                    if (context.Request.ContentType != null)
                    {
                        requestMessage.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(context.Request.ContentType);
                    }
                }
                
                // Send request to backend api
                var responseMessage = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
                
                // Copy response status
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                
                // Filter hop-by-hop/transport response headers to avoid chunked encoding issues
                var responseHeadersToSkip = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
                {
                    "Transfer-Encoding",
                    "Content-Length",
                    "Connection",
                    "Keep-Alive",
                    "Upgrade",
                    "Proxy-Authenticate",
                    "Proxy-Authorization",
                    "TE",
                    "Trailers"
                };

                foreach (var header in responseMessage.Headers)
                {
                    if (responseHeadersToSkip.Contains(header.Key))
                    {
                        continue;
                    }
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }
                foreach (var header in responseMessage.Content.Headers)
                {
                    if (responseHeadersToSkip.Contains(header.Key))
                    {
                        continue;
                    }
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }
                
                // Copy response body
                await responseMessage.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"[BFFProxy Error] path={proxyPath}, error={ex.Message}\n{ex.StackTrace}");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { Error = ex.Message, Details = ex.ToString() });
            }
        });
    }
}