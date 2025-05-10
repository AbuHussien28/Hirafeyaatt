using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.CustomerRepository
{
    public class OrderCustomerRepository:IOrderCustomerRepository
    {
        private readonly HirafeyatContext context;

        public OrderCustomerRepository(HirafeyatContext context)
        {
            this.context = context;
        }
        public async Task<List<CartItem>> GetCartItemsByUserIdAsync(string userId)
        {
            return await context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            await context.Orders.AddAsync(order);
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await context.Payments.AddAsync(payment);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId, string userId)
        {
            return await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == userId);
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await context.Orders
          .Include(o => o.OrderItems)
          .ThenInclude(oi => oi.Product)
          .Include(o => o.Payment) // Add this line to include payment
          .Where(o => o.CustomerId == userId)
          .OrderByDescending(o => o.OrderDate)
          .ToListAsync();
        }

        public void RemoveCartItems(List<CartItem> cartItems)
        {
            context.CartItems.RemoveRange(cartItems);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
