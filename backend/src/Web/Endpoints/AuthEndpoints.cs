using backend.Application.Auth.Commands.Register;
using backend.Application.Auth.Commands.Login;
using backend.Application.Auth.Commands.Logout;
using backend.Application.Auth.Commands.Refresh;
using backend.Application.Auth.Commands.ForgotPassword;
using backend.Application.Auth.Commands.ResetPassword;
using backend.Application.Auth.Commands.ChangePassword;
using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Infrastructure.Jwt;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
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
            groupBuilder.MapGet("me", GetMe).RequireAuthorization();
            groupBuilder.MapPost("logout", Logout).RequireAuthorization();
            groupBuilder.MapPost("forgot-password", ForgotPassword).AllowAnonymous();
            groupBuilder.MapPost("reset-password", ResetPassword).AllowAnonymous();
            groupBuilder.MapPost("change-password", ChangePassword).RequireAuthorization();
        }

        public static async Task<Results<Ok<ApiResponse<AuthResponse>>, BadRequest<string>>> Register(
            ISender sender,
            IIdentityService identityService,
            IOptions<JwtSettings> jwtOptions,
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
                    return TypedResults.BadRequest("Duplicated register request.");
                }
            }

            var result = await sender.Send(request);

            if (result is null)
            {
                return TypedResults.BadRequest("Registration failed.");
            }

            var user = await identityService.GetUserAsync(result.UserId);
            if (user is null) return TypedResults.BadRequest("User created but not found.");

            AppendRefreshTokenCookie(httpContext.Response, result.RefreshToken, jwtOptions.Value.RefreshTokenExpiryDays);

            var response = new AuthResponse
            {
                User = user,
                Tokens = new AuthTokens
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    ExpiresIn = jwtOptions.Value.ExpiryMinutes * 60
                }
            };

            return TypedResults.Ok(ApiResponse<AuthResponse>.Succeeded(response));
        }

        public static async Task<Results<Ok<ApiResponse<AuthResponse>>, UnauthorizedHttpResult, StatusCodeHttpResult>> Login(
            ISender sender,
            IIdentityService identityService,
            IOptions<JwtSettings> jwtOptions,
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

            var user = await identityService.GetUserAsync(result.UserId);
            if (user is null) return TypedResults.Unauthorized();

            AppendRefreshTokenCookie(httpContext.Response, result.RefreshToken, jwtOptions.Value.RefreshTokenExpiryDays);

            var response = new AuthResponse
            {
                User = user,
                Tokens = new AuthTokens
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    ExpiresIn = jwtOptions.Value.ExpiryMinutes * 60
                }
            };

            return TypedResults.Ok(ApiResponse<AuthResponse>.Succeeded(response));
        }

        public static async Task<Results<Ok<ApiResponse<AuthResponse>>, UnauthorizedHttpResult>> Refresh(
            ISender sender, 
            IIdentityService identityService,
            IOptions<JwtSettings> jwtOptions,
            HttpContext httpContext,
            RefreshTokenCommand? request)
        {
            var refreshToken = request?.RefreshToken ?? httpContext.Request.Cookies["refresh_token"];
            var accessToken = request?.AccessToken ?? httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(accessToken))
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

            var user = await identityService.GetUserAsync(result.UserId);
            if (user is null) return TypedResults.Unauthorized();

            AppendRefreshTokenCookie(httpContext.Response, result.RefreshToken, jwtOptions.Value.RefreshTokenExpiryDays);

            var response = new AuthResponse
            {
                User = user,
                Tokens = new AuthTokens
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    ExpiresIn = jwtOptions.Value.ExpiryMinutes * 60
                }
            };

            return TypedResults.Ok(ApiResponse<AuthResponse>.Succeeded(response));
        }

        public static async Task<Results<Ok<ApiResponse<UserDto>>, UnauthorizedHttpResult>> GetMe(
            IIdentityService identityService,
            IUser currentUser)
        {
            var userId = currentUser.Id;
            if (string.IsNullOrEmpty(userId)) return TypedResults.Unauthorized();

            var user = await identityService.GetUserAsync(userId);
            return user is null 
                ? TypedResults.Unauthorized() 
                : TypedResults.Ok(ApiResponse<UserDto>.Succeeded(user));
        }

        public static async Task<Results<Ok, UnauthorizedHttpResult>> Logout(
            ISender sender,
            HttpContext httpContext)
        {
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

        private static void AppendRefreshTokenCookie(HttpResponse response, string refreshToken, int expiryDays)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(expiryDays),
                Path = "/"
            };

            response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }
    }
}
