using backend.Domain.Enums;
namespace backend.Domain.Entities
{
    public class MoviePerson 
    {
        public int MovieId { get; set; }
        public int PersonId { get; set; }
        public RoleType RoleType { get; set; }

        public Movie Movie { get; set; } = null!;
        public Person Person { get; set; } = null!;
    }
}