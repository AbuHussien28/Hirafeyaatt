using Hirafeyat.Models;

namespace Hirafeyat.ViewModel
{
    public class SellerOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; }

        public OrderStatus Status { get; set; }
        public List<SellerOrderItemViewModel> Items { get; set; }
    }
}
