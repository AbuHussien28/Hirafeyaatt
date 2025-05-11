using Hirafeyat.ViewModel.OrderCustomer;

namespace Hirafeyat.CustomerRepository
{
    public interface IOrderCustomerService
    {
        Task<CheckoutPageViewModel> PrepareCheckoutAsync(string userId);
        Task<string> ProcessOrderAsync(string userId, CheckoutPageViewModel model, string paymentMethod, string paymentIntentId);
        Task<List<OrderWithPaymentViewModel>> GetUserOrdersAsync(string userId);

        Task<bool> CancelOrderAsync(int orderId, string userId);
        Task CreateBasicOrderAsync(string userId, string email);
    }
}
