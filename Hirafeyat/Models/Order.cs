using System.ComponentModel.DataAnnotations.Schema;

namespace Hirafeyat.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public  ApplicationUser Customer { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        
    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

}
