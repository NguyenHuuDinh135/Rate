using backend.Application.Common.Interfaces;
using backend.Application.Common.Models;
using backend.Application.Users.Commands.UpdateUser;
using backend.Application.Users.Queries.GetMyUserInfo;
using backend.Application.Users.Queries.GetUsers;
using backend.Application.Users.Queries.GetUserById;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class UserEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/users";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetMe).RequireAuthorization();
        group.MapGet("/all", GetAll).RequireAuthorization(); // Admin only policy could be added here
        group.MapGet("/id/{id}", GetById).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
    }

    public static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> GetMe(ISender sender)
    {
        var result = await sender.Send(new GetMyUserInfoQuery());
        return result is null ? TypedResults.Unauthorized() : TypedResults.Ok(result);
    }

    public static Task<IReadOnlyList<UserDto>> GetAll(ISender sender)
        => sender.Send(new GetUsersQuery());

    public static async Task<Results<Ok<UserDto>, NotFound>> GetById(ISender sender, string id)
    {
        var result = await sender.Send(new GetUserByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdateUserCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
