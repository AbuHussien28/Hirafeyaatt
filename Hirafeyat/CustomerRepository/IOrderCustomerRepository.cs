namespace Hirafeyat.CustomerRepository
{
    public interface IOrderCustomerRepository
    {
        Task<List<CartItem>> GetCartItemsByUserIdAsync(string userId);
        Task AddOrderAsync(Order order);
        Task AddPaymentAsync(Payment payment);
        Task<Order> GetOrderByIdAsync(int orderId, string userId);
        Task<List<Order>> GetUserOrdersAsync(string userId);
        Task CreateOrderWithCartItemsAsync(string userId, string email);
        Task SaveChangesAsync();
        void RemoveCartItems(List<CartItem> cartItems);
    }
}
