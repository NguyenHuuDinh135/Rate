namespace WebUI.Shared.Models;

public class Movie
{
    public string Title { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public string Type { get; set; } = "Phim rạp";
    public bool IsUpcoming { get; set; }
    public string Status { get; set; } = string.Empty;
}
