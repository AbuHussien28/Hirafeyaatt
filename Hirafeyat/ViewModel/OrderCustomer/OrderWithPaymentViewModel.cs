

namespace Hirafeyat.ViewModel.OrderCustomer
{
    public class OrderWithPaymentViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
    }
}
