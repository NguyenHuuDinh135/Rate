using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pgvector;
using System.Collections.Generic;

public enum MovieType { ComingSoon, NowShowing, Removed }
public class Movie {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal? Rating { get; set; }
    public string TrailerUrl { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public MovieType MovieType { get; set; }
    public Vector? Embedding { get; set; }
}

class Program {
    static void Main() {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        try {
            var path = "/Users/nqvinh/Documents/projects/Rate/backend/src/Infrastructure/Data/SeedData/Movies.json";
            var json = File.ReadAllText(path);
            var movies = JsonSerializer.Deserialize<Movie[]>(json, options);
            Console.WriteLine("Success: " + (movies != null ? movies.Length.ToString() : "null"));
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
