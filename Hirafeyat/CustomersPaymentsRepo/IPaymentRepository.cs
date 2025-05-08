using Hirafeyat.Models;

namespace Hirafeyat.CustomersPaymentsRepo
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Order> GetOrderWithCustomerAsync(int id);
    }
}
