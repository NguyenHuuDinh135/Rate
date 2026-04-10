using backend.Application.Users.Queries.GetMyUserInfo;
using backend.Application.Users.Queries.GetUserById;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class UserEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/users";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetMe).RequireAuthorization();
        group.MapGet("/id/{id}", GetById).RequireAuthorization();
    }

    public static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> GetMe(ISender sender)
    {
        var result = await sender.Send(new GetMyUserInfoQuery());
        return result is null ? TypedResults.Unauthorized() : TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<UserDto>, NotFound>> GetById(ISender sender, string id)
    {
        var result = await sender.Send(new GetUserByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}
