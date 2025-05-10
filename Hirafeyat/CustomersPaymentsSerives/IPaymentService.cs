using Hirafeyat.Models;
using Hirafeyat.ViewModel.Payments;

namespace Hirafeyat.CustomersPaymentsSerives
{
    public interface IPaymentService
    {
        Task HandleStripePaymentAsync(PaymentViewModel paymentViewModel);
        Task <PaymentViewModel> GetByIdAsync(int id);
        Task<PaymentResult> ProcessPaymentAsync(int orderId, string paymentIntentId);
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int OrderId { get; set; }
    }
}
