using backend.Application.Auth.Commands.Register;
using backend.Application.Auth.Commands.Login;
using backend.Application.Auth.Commands.Logout;
using backend.Application.Auth.Commands.Refresh;
using backend.Application.Auth.Commands.ForgotPassword;
using backend.Application.Auth.Commands.ResetPassword;
using backend.Application.Auth.Commands.ChangePassword;
using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace backend.Web.Endpoints
{
    public class AuthEndpoints : IEndpointGroup
    {
        public static string RoutePrefix => "/api/auth";

        public static void Map(RouteGroupBuilder groupBuilder)
        {
            groupBuilder.MapPost("register", Register).AllowAnonymous();
            groupBuilder.MapPost("login", Login).AllowAnonymous();
            groupBuilder.MapPost("refresh", Refresh).AllowAnonymous();
            groupBuilder.MapPost("logout", Logout).RequireAuthorization();
            groupBuilder.MapPost("forgot-password", ForgotPassword).AllowAnonymous();
            groupBuilder.MapPost("reset-password", ResetPassword).AllowAnonymous();
            groupBuilder.MapPost("change-password", ChangePassword).RequireAuthorization();
            groupBuilder.MapGet("external-login", ExternalLogin).AllowAnonymous();
            groupBuilder.MapGet("external-callback", ExternalCallback).AllowAnonymous();
        }

        public static async Task<Results<Ok<Result>, BadRequest<Result>, Conflict<string>>> Register(
            ISender sender,
            RegisterRequest request,
            HttpContext httpContext,
            IIdempotencyService idempotencyService)
        {
            var idemKey = httpContext.Request.Headers["Idempotency-Key"].ToString();
            if (!string.IsNullOrWhiteSpace(idemKey))
            {
                var acquired = await idempotencyService.TryAcquireAsync($"register:{idemKey}", TimeSpan.FromMinutes(5), httpContext.RequestAborted);
                if (!acquired)
                {
                    return TypedResults.Conflict("Duplicated register request.");
                }
            }

            var result = await sender.Send(request);

            if (!result.Succeeded)
            {
                return TypedResults.BadRequest(result);
            }

            return TypedResults.Ok(result);
        }

        private static void SetTokenCookies(HttpContext httpContext, AuthTokenResult result)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Force secure in production/local if using HTTPS
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };

            httpContext.Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = cookieOptions.Secure,
                SameSite = cookieOptions.SameSite,
                Expires = DateTimeOffset.UtcNow.AddHours(2),
                Path = "/"
            });

            httpContext.Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = cookieOptions.Secure,
                SameSite = cookieOptions.SameSite,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            });
        }

        public static async Task<Results<Ok<AuthTokenResult>, UnauthorizedHttpResult, StatusCodeHttpResult>> Login(
            ISender sender,
            LoginCommand request,
            HttpContext httpContext,
            IRateLimitService rateLimitService)
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var allowed = await rateLimitService.IsAllowedAsync(
                $"login:{ip}:{request.Email.ToLowerInvariant()}",
                limit: 8,
                window: TimeSpan.FromMinutes(1),
                httpContext.RequestAborted);

            if (!allowed)
            {
                return TypedResults.StatusCode(StatusCodes.Status429TooManyRequests);
            }

            var result = await sender.Send(request);

            if (result is null)
            {
                return TypedResults.Unauthorized();
            }

            SetTokenCookies(httpContext, result);

            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<AuthTokenResult>, UnauthorizedHttpResult>> Refresh(
            ISender sender, 
            HttpContext httpContext,
            RefreshTokenCommand? requestBody)
        {
            string? accessToken = httpContext.Request.Cookies["access_token"];
            string? refreshToken = httpContext.Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = requestBody?.AccessToken;
                if (string.IsNullOrEmpty(accessToken))
                {
                    var authHeader = httpContext.Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        accessToken = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                refreshToken = requestBody?.RefreshToken;
            }

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return TypedResults.Unauthorized();
            }

            var result = await sender.Send(new RefreshTokenCommand
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });

            if (result is null)
            {
                return TypedResults.Unauthorized();
            }

            SetTokenCookies(httpContext, result);

            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok, UnauthorizedHttpResult>> Logout(
            ISender sender,
            HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("access_token");
            httpContext.Response.Cookies.Delete("refresh_token");

            var principal = httpContext.User;
            var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrWhiteSpace(jti))
            {
                return TypedResults.Unauthorized();
            }

            var expValue = principal.FindFirstValue(JwtRegisteredClaimNames.Exp);
            if (!long.TryParse(expValue, out var expUnix))
            {
                return TypedResults.Unauthorized();
            }

            var ok = await sender.Send(new LogoutCommand
            {
                Jti = jti,
                ExpiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix)
            });

            return ok ? TypedResults.Ok() : TypedResults.Unauthorized();
        }

        public static IResult ExternalLogin(string provider, HttpContext httpContext)
        {
            var callbackUrl = $"/api/auth/external-callback?provider={provider}&email={provider.ToLower()}user@example.com&username={provider.ToLower()}user";
            return TypedResults.Redirect(callbackUrl);
        }

        public static async Task<IResult> ExternalCallback(
            string provider,
            string email,
            string username,
            IAuthenticationService authService,
            HttpContext httpContext)
        {
            var result = await authService.ExternalLoginAsync(email, username, httpContext.RequestAborted);
            if (result is null)
            {
                return TypedResults.Redirect("/auth/login?error=ExternalLoginFailed");
            }

            SetTokenCookies(httpContext, result);

            return TypedResults.Redirect("/");
        }

        public static async Task<Ok<ForgotPasswordResponse>> ForgotPassword(
            ISender sender,
            ForgotPasswordCommand request)
        {
            var result = await sender.Send(request);
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<Result>, BadRequest<Result>>> ResetPassword(
            ISender sender,
            ResetPasswordCommand request)
        {
            var result = await sender.Send(request);
            return result.Succeeded
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }

        public static async Task<Results<Ok<Result>, BadRequest<Result>, UnauthorizedHttpResult>> ChangePassword(
            ISender sender,
            ChangePasswordCommand request,
            HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                         ?? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return TypedResults.Unauthorized();
            }

            var result = await sender.Send(request with { UserId = userId });
            return result.Succeeded
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}