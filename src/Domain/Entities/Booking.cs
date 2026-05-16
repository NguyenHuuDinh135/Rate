
using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int ShowId { get; set; }
        public string SeatRow { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public float Price { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime BookingDateTime { get; set; }

        public Show Show { get; set; } = null!;
    }
}