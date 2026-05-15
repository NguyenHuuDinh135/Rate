using backend.Infrastructure.Persistence.Dapper;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Http.HttpResults;
using backend.Application.Movies.Queries.GetMovies;
using backend.Application.Movies.Queries.GetFilteredMovies;
using Microsoft.AspNetCore.Mvc;

namespace backend.Web.Endpoints;

public class Movies : IEndpointGroup
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/top", GetTopMovies)
            .WithName("GetTopMovies")
            .WithTags("Movies");

        group.MapGet("/search", SearchMovies)
            .WithName("SearchMovies")
            .WithTags("Movies");
    }

    public static async Task<Ok<IEnumerable<MovieDto>>> GetTopMovies(
        MovieDapperRepository repo)
    {
        var movies = await repo.GetTopMoviesAsync();
        return TypedResults.Ok(movies);
    }

    public static async Task<IResult> SearchMovies(
        [FromQuery] string? q,
        ISender sender,
        ElasticsearchClient elastic)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Results.Ok(Enumerable.Empty<MovieBriefDto>());
        }

        // Primary search: Use Database via existing GetFilteredMoviesQuery
        var movies = await sender.Send(new GetFilteredMoviesQuery(q, null, null));
        
        if (movies != null && movies.Any())
        {
            return Results.Ok(movies);
        }

        // Fallback: Elasticsearch
        try 
        {
            var response = await elastic.SearchAsync<MovieBriefDto>(s => s
                .Indices("movies")
                .Query(qry => qry
                    .Match(m => m.Field("title").Query(q))
                )
            );

            if (response.IsValidResponse && response.Documents.Any())
            {
                return Results.Ok(response.Documents);
            }
        }
        catch 
        {
            // Silent fail
        }

        return Results.Ok(Enumerable.Empty<MovieBriefDto>());
    }
}