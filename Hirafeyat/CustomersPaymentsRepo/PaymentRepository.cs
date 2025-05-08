using Hirafeyat.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Order> GetOrderWithCustomerAsync(int id)
        {
            return await context.Orders.Include(o=>o.Customer).Include(p=>p.Product).FirstOrDefaultAsync(i=>i.Id==id);
              
        }
    }
}
