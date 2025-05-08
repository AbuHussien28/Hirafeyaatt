using Hirafeyat.Models;
using Hirafeyat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.SellerServices
{
    public class OrderService : IOrderService
    {
        private readonly HirafeyatContext _context;

        public OrderService(HirafeyatContext context)
        {
            this._context = context;
        }


        public List<Order> getAll()
        {
            return _context.Orders.ToList();
        }

        public Order getById(int id)
        {
            return _context.Orders
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Category)
        .Include(o => o.Customer)
        .FirstOrDefault(o => o.Id == id);

            

        }




        public void add(Order item)
        {
            _context.Orders.Add(item);
        }
        public void update(Order item)
        {
            _context.Orders.Update(item);
        }
        public void delete(Order item)
        {
            _context.Orders.Remove(item);
        }

        public List<Order> GetAllOrdersBySellerId(string seller_id)
        {
            return _context.Orders
               //.Where(o => o.Product.SellerId == seller_id)
               //.Include(o => o.Product)
                   //.ThenInclude(p => p.Category)
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


        public int save()
        {

            return _context.SaveChanges();

        }

    }
}
