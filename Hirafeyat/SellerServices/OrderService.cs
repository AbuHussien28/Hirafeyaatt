using Hirafeyat.Models;
using Hirafeyat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.SellerServices
{
    public class OrderService: IOrderService
    {
        private readonly HirafeyatContext _context;

        public OrderService(HirafeyatContext context)
        {
            this._context = context;
        }

        public Order GetOrderById(int id) 
        {
            return _context.Orders
                    .Where(o => o.Id == id)
                    .Include(o => o.Product)
                    .ThenInclude(p => p.Category)
                    .Include(o => o.Customer)
                    .FirstOrDefault();
        
        }
        public List<Order> GetAllOrdersBySellerId(string seller_id)
        {
            return _context.Orders
               .Where(o => o.Product.SellerId == seller_id)
               .Include(o => o.Product) 
                   .ThenInclude(p => p.Category) 
               .Include(o => o.Customer) 
               .ToList();
        }
  
     
        public void UpdateOrderStatus(int orderId, OrderStatus newStatus) 
        {
         var order = _context.Orders.Find(orderId);
            if (order != null) 
            {
                order.Status = newStatus;
                _context.Orders.Update(order);
            }

        }
        public void Save() 
        {
        
         _context.SaveChanges();
        
        }

    }
}
