using Hirafeyat.Models;

namespace Hirafeyat.ViewModel
{
    public class SellerOrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int Quantity { get; set; }
        public OrderStatus ProductStatus { get; set; }
    }
}
