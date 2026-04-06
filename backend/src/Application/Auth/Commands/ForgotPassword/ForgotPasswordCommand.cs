using backend.Application.Common.Interfaces;

namespace backend.Application.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public string Email { get; init; } = string.Empty;
}

public sealed class ForgotPasswordResponse
{
    public string Message { get; init; } = "If the email exists, password reset instructions have been sent.";
}

public sealed class ForgotPasswordCommandHandler(
    IAuthenticationService authenticationService,
    IEmailSender emailSender)
    : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var token = await authenticationService.CreatePasswordResetTokenAsync(request.Email, cancellationToken);

        if (!string.IsNullOrWhiteSpace(token))
        {
            var resetLink = $"https://localhost:3000/reset-password?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(token)}";
            var body = $"""
                        <p>We received a password reset request.</p>
                        <p>Use this code: <strong>{token}</strong></p>
                        <p>Or click: <a href="{resetLink}">{resetLink}</a></p>
                        <p>This code expires in 15 minutes.</p>
                        """;

            await emailSender.SendAsync(
                request.Email,
                "Reset your password",
                body,
                cancellationToken);
        }

        return new ForgotPasswordResponse();
    }
}

