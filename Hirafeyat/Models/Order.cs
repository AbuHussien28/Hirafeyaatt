using System.ComponentModel.DataAnnotations.Schema;

namespace Hirafeyat.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public  Product Product { get; set; }
        
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public  ApplicationUser Customer { get; set; }
        
        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        //was string now enum
        public OrderStatus Status { get; set; }

        public string Address { get; set; }

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
