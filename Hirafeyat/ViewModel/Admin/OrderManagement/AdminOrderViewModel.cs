using Hirafeyat.Models;

namespace Hirafeyat.ViewModel.Admin.OrderManagement
{
    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string SellerFullName { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerEmail { get; set; }
        public string ProductTitle { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
