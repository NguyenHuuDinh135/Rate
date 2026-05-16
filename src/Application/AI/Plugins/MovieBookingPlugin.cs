using System.ComponentModel;
using backend.Application.Movies.Queries.GetFilteredMovies;
using backend.Application.Shows.Queries.GetFilteredShows;
using MediatR;
using Microsoft.SemanticKernel;

namespace backend.Application.AI.Plugins;

public class MovieBookingPlugin(ISender sender)
{
    [KernelFunction("search_movies")]
    [Description("Tìm kiếm phim theo tên, thể loại hoặc năm sản xuất.")]
    public async Task<string> SearchMovies(
        [Description("Tên phim cần tìm")] string? title = null,
        [Description("Năm sản xuất")] int? year = null)
    {
        var result = await sender.Send(new GetFilteredMoviesQuery(title, null, year));

        if (result == null || !result.Any()) return "Không tìm thấy phim nào phù hợp.";

        return string.Join("\n", result.Select(m => $"- {m.Title} (ID: {m.Id}, Năm: {m.Year}): {m.Summary}"));
    }

    [KernelFunction("get_showtimes")]
    [Description("Lấy lịch chiếu của một bộ phim.")]
    public async Task<string> GetShowtimes(
        [Description("ID của phim")] int movieId,
        [Description("Ngày chiếu (yyyy-MM-dd)")] string? date = null)
    {
        DateTime? parsedDate = null;
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d))
        {
            parsedDate = d;
        }

        var result = await sender.Send(new GetFilteredShowsQuery(parsedDate, movieId));

        if (result == null || !result.Any()) return "Không có lịch chiếu nào cho phim này vào thời gian đã chọn.";

        return string.Join("\n", result.Select(s => $"- Suất chiếu ID {s.Id}: {s.StartTime:hh\\:mm} tại rạp {s.TheaterName} (Ngày: {s.Date:dd/MM/yyyy}, Giá: {s.Price}đ)"));
    }
}
