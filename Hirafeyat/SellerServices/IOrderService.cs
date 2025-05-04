using Hirafeyat.Models;

namespace Hirafeyat.Services
{
    public interface IOrderService
    {
        Order GetOrderById(int id);
        //List<Order> GetAllOrders();
        List<Order> GetAllOrdersBySellerId(string stringId);
        void UpdateOrderStatus(int orderId, OrderStatus newStatus);
       

        void Save();


    }
}
