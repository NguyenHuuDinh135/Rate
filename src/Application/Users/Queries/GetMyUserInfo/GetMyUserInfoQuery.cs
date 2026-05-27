using backend.Application.Common.Interfaces;

namespace backend.Application.Users.Queries.GetMyUserInfo;

public sealed record UserDto(string Id, string UserName, string Email, List<string> Roles);

public sealed record GetMyUserInfoQuery : IRequest<UserDto?>;

public sealed class GetMyUserInfoQueryHandler(IUser user, IIdentityService identityService)
    : IRequestHandler<GetMyUserInfoQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetMyUserInfoQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user.Id)) return null;
        var userName = await identityService.GetUserNameAsync(user.Id);
        if (userName is null) return null;
        
        var roles = await identityService.GetRolesAsync(user.Id);
        
        return new UserDto(user.Id, userName, userName, roles.ToList());
    }
}
