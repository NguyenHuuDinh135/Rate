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

            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<AuthTokenResult>, UnauthorizedHttpResult>> Refresh(ISender sender, RefreshTokenCommand request)
        {
            var result = await sender.Send(request);
            if (result is null)
            {
                return TypedResults.Unauthorized();
            }

            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok, UnauthorizedHttpResult>> Logout(
            ISender sender,
            HttpContext httpContext)
        {
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

        // public static async Task<IResult> RefreshToken(ISender sender, RefreshTokenRequest request)
        // {
        //     var result = await sender.Send(request);

        //     return result.Match(
        //         authResult => Results.Ok(authResult),
        //         error => Results.BadRequest(error));
        // }

        // public static async Task<IResult> ForgotPassword(ISender sender, ForgotPasswordRequest request)
        // {
        //     var result = await sender.Send(request);

        //     return result.Match(
        //         _ => Results.Ok(),
        //         error => Results.BadRequest(error));
        // }

        // public static async Task<IResult> ChangePassword(ISender sender, ChangePasswordRequest request)
        // {
        //     var result = await sender.Send(request);

        //     return result.Match(
        //         _ => Results.Ok(),
        //         error => Results.BadRequest(error));
        // }

        // public static async Task<IResult> Logout(ISender sender, LogoutRequest request)
        // {
        //     var result = await sender.Send(request);

        //     return result.Match(
        //         _ => Results.Ok(),
        //         error => Results.BadRequest(error));
    }
}