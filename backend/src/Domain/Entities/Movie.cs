using System.Collections.Generic;

namespace backend.Domain.Entities
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal? Rating { get; set; }
        public string TrailerUrl { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public MovieType MovieType { get; set; }

        public ICollection<Show> Shows { get; set; } = new List<Show>();
        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public ICollection<MoviePerson> MoviePersons { get; set; } = new List<MoviePerson>();
    }
}