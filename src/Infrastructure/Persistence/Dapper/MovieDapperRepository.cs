using Dapper;
using Dapper.AOT;
using Microsoft.Extensions.Configuration;
using Npgsql;
using backend.Application.Movies.Queries.GetMovies;

namespace backend.Infrastructure.Persistence.Dapper;

// Sử dụng đúng Attribute cho phiên bản 1.0.48+
[DapperAot]
public partial class MovieDapperRepository // Bắt buộc phải có partial
{
    private readonly string _connectionString;

    public MovieDapperRepository(IConfiguration configuration)
    {
        // MovieDb là tên bạn đặt trong AppHost
        _connectionString = configuration.GetConnectionString("MovieDb")!;
    }

    public async Task<IEnumerable<MovieDto>> GetAllAsync()
    {
        // Dùng nháy kép cho Postgres để khớp với DB đã Seed
        const string sql = """
            SELECT "Id", "Title", "Year", "Rating"
            FROM "Movies"
            ORDER BY "Year" DESC
            """;

        using var connection = new NpgsqlConnection(_connectionString);
        
        // CommandDefinition giúp Dapper.AOT tối ưu ánh xạ dữ liệu tĩnh
        var command = new CommandDefinition(sql);

        // Trình tạo mã sẽ tự sinh code cho phương thức này
        return await connection.QueryAsync<MovieDto>(command);
    }

    public async Task<IEnumerable<MovieDto>> GetTopMoviesAsync()
    {
        const string sql = """
            SELECT "Id", "Title", "Year", "Rating"
            FROM "Movies"
            ORDER BY "Rating" DESC
            LIMIT 10
            """;

        using var connection = new NpgsqlConnection(_connectionString);

        var command = new CommandDefinition(sql);

        return await connection.QueryAsync<MovieDto>(command);
    }
}