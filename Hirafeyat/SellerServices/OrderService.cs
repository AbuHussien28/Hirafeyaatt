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
    .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.Category)
            .Include(o => o.Customer)
            .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == seller_id))
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

        public void UpdateOrderStatusByProducts(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null) return;

            var statuses = order.OrderItems.Select(oi => oi.ItemStatus).ToList();

            if (statuses.All(s => s == OrderStatus.Delivered))
            {
                order.Status = OrderStatus.Delivered;
            }
            else if (statuses.All(s => s == OrderStatus.Shipped || s == OrderStatus.Delivered))
            {
                order.Status = OrderStatus.Shipped;
            }
            else
            {
                order.Status = OrderStatus.Processing;
            }

            _context.Orders.Update(order);
        }

        public IQueryable<Order> getAllWithoutLoading()
        {
            return _context.Orders ;
        }

        public int save()
        {

            return _context.SaveChanges();

        }

    }
}
