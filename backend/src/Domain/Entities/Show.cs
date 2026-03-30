using System.Collections.Generic;
using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    public class Show : BaseEntity
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime Date { get; set; }
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public ShowStatus Status { get; set; }
        public ShowType Type { get; set; }

        public Movie Movie { get; set; } = null!;
        public Theater Theater { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}