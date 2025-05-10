using Hirafeyat.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Climate;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;
using Order = Hirafeyat.Models.Order;

namespace Hirafeyat.CustomersPaymentsRepo
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly HirafeyatContext context;

        public PaymentRepository(HirafeyatContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(Payment payment)
        {
            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();
        }

        public async Task<OrderItem> GetOrderWithCustomerAsync(int id)
        {
            return await context.OrderItems.Include(o => o.Order)
                .ThenInclude(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

        }
        public async Task UpdateOrderAndPaymentStatusAsync(int orderId, OrderStatus orderStatus,
            PaymentStatus paymentStatus, string paymentIntentId = null)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = orderStatus;
            }

            var payment = await context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment != null)
            {
                payment.Status = paymentStatus;
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    payment.StripePaymentIntentId = paymentIntentId;
                }
            }

            await context.SaveChangesAsync();
        }

    }
}
