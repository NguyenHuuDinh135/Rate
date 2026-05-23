using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pgvector;

public enum MovieType { ComingSoon, NowShowing, Removed }
public class Movie {
    public string Title { get; set; }
    public MovieType MovieType { get; set; }
    public Vector? Embedding { get; set; }
}

class Program {
    static void Main() {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        try {
            var movie = JsonSerializer.Deserialize<Movie>("{\"Title\":\"Test\"}", options);
            Console.WriteLine("Success: " + movie.Title);
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
