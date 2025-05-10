namespace Hirafeyat.ViewModel.Payments
{
    public class CheckoutViewModel
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
