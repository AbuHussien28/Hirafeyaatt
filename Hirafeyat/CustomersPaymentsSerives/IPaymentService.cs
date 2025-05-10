using Hirafeyat.Models;
using Hirafeyat.ViewModel.Payments;

namespace Hirafeyat.CustomersPaymentsSerives
{
    public interface IPaymentService
    {
        Task HandleStripePaymentAsync(CheckoutViewModel paymentViewModel);
        Task <CheckoutViewModel> GetByIdAsync(int id);
    }
}
