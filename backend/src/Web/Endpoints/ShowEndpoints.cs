using backend.Application.Common.Models;
using backend.Application.Shows.Commands.CreateShow;
using backend.Application.Shows.Commands.DeleteShow;
using backend.Application.Shows.Commands.UpdateShow;
using backend.Application.Shows.Queries.GetFilteredShows;
using backend.Application.Shows.Queries.GetShowById;
using backend.Application.Shows.Queries.GetShows;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Web.Endpoints;

public class ShowEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/shows";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/all", GetAll).AllowAnonymous();
        group.MapGet("/id/{id:int}", GetById).AllowAnonymous();
        group.MapGet("/filters", GetFiltered).AllowAnonymous();
        group.MapPost("/create", Create).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/id/{id:int}", Delete).RequireAuthorization();
    }

    public static Task<IReadOnlyList<ShowBriefDto>> GetAll(ISender sender)
        => sender.Send(new GetShowsQuery());

    public static async Task<Results<Ok<ShowBriefDto>, NotFound>> GetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetShowByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static Task<IReadOnlyList<ShowDetailDto>> GetFiltered(
        ISender sender, 
        [FromQuery(Name = "date")] string? dateStr, 
        int? movieId)
    {
        DateTime? date = null;
        if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out var d))
        {
            date = d;
        }
        return sender.Send(new GetFilteredShowsQuery(date, movieId));
    }

    public static async Task<Results<Ok<Result<int>>, BadRequest<Result<int>>>> Create(
        ISender sender, CreateShowCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdateShowCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Delete(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteShowCommand(id));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
