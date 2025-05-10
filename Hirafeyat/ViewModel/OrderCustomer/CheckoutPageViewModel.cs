namespace Hirafeyat.ViewModel.OrderCustomer
{
    public class CheckoutPageViewModel
    {
        public CheckoutViewModel CheckoutForm { get; set; } = new CheckoutViewModel();
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }
        public string ClientSecret { get; set; }
        public string StripePublishableKey { get; set; }
    }
}
