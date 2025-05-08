namespace Hirafeyat.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

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

//[Key]
//public int Id { get; set; }

//public int ProductId { get; set; }
//public Product Product { get; set; }

//public string CustomerId { get; set; }
//public ApplicationUser Customer { get; set; }

//public int Quantity { get; set; }

//public DateTime OrderDate { get; set; } = DateTime.Now;


//public OrderStatus Status { get; set; }

//public string Address { get; set; }
