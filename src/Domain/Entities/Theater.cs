using System.Collections.Generic;
using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    public class Theater : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int NumOfRows { get; set; }
        public int SeatsPerRow { get; set; }
        public TheaterType Type { get; set; }

        public ICollection<Show> Shows { get; set; } = new List<Show>();
        public ICollection<TheaterSeat> TheaterSeats { get; set; } = new List<TheaterSeat>();
    }
}