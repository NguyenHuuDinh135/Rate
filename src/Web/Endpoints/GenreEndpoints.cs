using backend.Application.Common.Models;
using backend.Application.Genres.Commands.CreateGenre;
using backend.Application.Genres.Commands.DeleteGenre;
using backend.Application.Genres.Commands.UpdateGenre;
using backend.Application.Genres.Queries.GetGenreById;
using backend.Application.Genres.Queries.GetGenres;
using backend.Application.Genres.Queries.GetGenresByMovie;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class GenreEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/genres";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/all", GetAll).AllowAnonymous();
        group.MapGet("/id/{id:int}", GetById).RequireAuthorization();
        group.MapGet("/movies/{movieId:int}", GetByMovie).AllowAnonymous();
        group.MapPost("/create", Create).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/delete/{id:int}", Delete).RequireAuthorization();
    }

    public static Task<IReadOnlyList<GenreBriefDto>> GetAll(ISender sender)
        => sender.Send(new GetGenresQuery());

    public static async Task<Results<Ok<GenreBriefDto>, NotFound>> GetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetGenreByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static Task<IReadOnlyList<GenreBriefDto>> GetByMovie(ISender sender, int movieId)
        => sender.Send(new GetGenresByMovieQuery(movieId));

    public static async Task<Results<Ok<Result<int>>, BadRequest<Result<int>>>> Create(
        ISender sender, CreateGenreCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdateGenreCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Delete(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteGenreCommand(id));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
