using backend.Application.Common.Interfaces;

namespace backend.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(string UserId) : IRequest<UserDto?>;

public sealed class GetUserByIdQueryHandler(IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var userName = await identityService.GetUserNameAsync(request.UserId);
        if (userName is null) return null;
        return new UserDto(request.UserId, userName, userName);
    }
}
