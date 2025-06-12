using System.ComponentModel.DataAnnotations.Schema;

namespace Hirafeyat.Models
{
 
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public PaymentStatus Status { get; set; }
        public string PaymentMethod { get; set; } 
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }

}
