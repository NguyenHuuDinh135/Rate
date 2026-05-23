using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Auth.Commands.Register
{
    public record RegisterRequest : IRequest<AuthTokenResult?>
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
    public class RegisterCommand : IRequestHandler<RegisterRequest, AuthTokenResult?>
    {
        public readonly IApplicationDbContext _dbContext;
        public readonly IIdentityService _identityService;
        private readonly IAuthenticationService _authService;
        public RegisterCommand(IApplicationDbContext dbContext, IIdentityService identityService, IAuthenticationService authService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _authService = authService;
        }

        public async Task<AuthTokenResult?> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _identityService.CreateUserAsync(
                request.FullName,
                request.Email,
                request.Password);

            if (!result.Result.Succeeded)
            {
                return null;
            }

            return await _authService.LoginAsync(request.Email, request.Password, cancellationToken);
        }
    }
}