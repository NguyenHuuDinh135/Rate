using backend.Infrastructure.Persistence.Dapper;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Http.HttpResults;
using backend.Application.Movies.Queries.GetMovies;

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
        string q,
        ElasticsearchClient elastic)
    {
        var response = await elastic.SearchAsync<dynamic>(s => s
            .Indices("movies")
            .Query(qry => qry
                .Match(m => m.Field("title").Query(q))
            )
        );

        return Results.Ok(response.Documents);
    }
}