using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    

    public class TheaterSeat
    {
        public string SeatRow { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public int TheaterId { get; set; }
        public SeatType Type { get; set; }

        public Theater Theater { get; set; } = null!;
    }
}