﻿namespace Hirafeyat.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public OrderStatus ItemStatus { get; set; } = OrderStatus.Pending;
    }
}
