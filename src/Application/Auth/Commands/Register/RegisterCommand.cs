using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;

namespace backend.Application.Auth.Commands.Register
{
    public record RegisterRequest : IRequest<Result>
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
    public class RegisterCommand : IRequestHandler<RegisterRequest, Result>
    {
        public readonly IApplicationDbContext _dbContext;
        public readonly IIdentityService _identityService;
        public RegisterCommand(IApplicationDbContext dbContext, IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
        }

        public async Task<Result> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _identityService.CreateUserAsync(
                request.FullName,
                request.Email,
                request.Password);

            if (!result.Result.Succeeded)
            {
                return Result.Failure(result.Result.Errors);
            }

            return Result.Success();
        }
    }
}
