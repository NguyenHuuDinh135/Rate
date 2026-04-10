using backend.Application.Common.Models;
using backend.Application.Theaters.Commands.CreateTheater;
using backend.Application.Theaters.Commands.DeleteTheater;
using backend.Application.Theaters.Commands.UpdateTheater;
using backend.Application.Theaters.Queries.GetTheaterById;
using backend.Application.Theaters.Queries.GetTheaters;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class TheaterEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/theaters";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll).AllowAnonymous();
        group.MapGet("/id/{id:int}", GetById).AllowAnonymous();
        group.MapPost("/", Create).RequireAuthorization();
        group.MapPut("/", Update).RequireAuthorization();
        group.MapDelete("/id/{id:int}", Delete).RequireAuthorization();
    }

    public static Task<IReadOnlyList<TheaterBriefDto>> GetAll(ISender sender)
        => sender.Send(new GetTheatersQuery());

    public static async Task<Results<Ok<TheaterBriefDto>, NotFound>> GetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetTheaterByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<Result<int>>, BadRequest<Result<int>>>> Create(
        ISender sender, CreateTheaterCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdateTheaterCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Delete(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteTheaterCommand(id));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
