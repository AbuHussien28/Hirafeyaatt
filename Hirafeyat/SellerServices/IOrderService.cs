using Hirafeyat.Models;
using Hirafeyat.SellerServices;

namespace Hirafeyat.Services
{
    public interface IOrderService : IRepository<Order>
    {

        List<Order> GetAllOrdersBySellerId(string stringId);
        void UpdateOrderStatus(int orderId, OrderStatus newStatus);
        //public List<Order> getAllWithSeller();


    }
}
