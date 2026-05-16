
using backend.Domain.Enums;

namespace backend.Domain.Entities
{
    

    public class Payment : BaseEntity
    {
        public int Amount { get; set; }
        public DateTime PaymentDateTime { get; set; }
        public PaymentMethod Method { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ShowId { get; set; }

        public Show Show { get; set; } = null!;
    }
}