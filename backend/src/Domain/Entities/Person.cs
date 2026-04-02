using System.Collections.Generic;

namespace backend.Domain.Entities
{
    public class Person : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public byte Age { get; set; }
        public string PictureUrl { get; set; } = string.Empty;

        public ICollection<MoviePerson> MoviePersons { get; set; } = new List<MoviePerson>();
    }
}