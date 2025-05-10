using Hirafeyat.Models;

namespace Hirafeyat.CustomersPaymentsRepo
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<OrderItem> GetOrderWithCustomerAsync(int id);
        Task UpdateOrderAndPaymentStatusAsync(int orderId, OrderStatus orderStatus, PaymentStatus paymentStatus, string paymentIntentId = null);
    }
}
