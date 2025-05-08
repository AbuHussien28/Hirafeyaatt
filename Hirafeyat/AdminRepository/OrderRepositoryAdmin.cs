using Hirafeyat.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EntityFramework;

namespace Hirafeyat.AdminRepository
{
    public class OrderRepositoryAdmin: IOrderRepositoryAdmin
    {
        private readonly HirafeyatContext _context;
        public OrderRepositoryAdmin(HirafeyatContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<Order>> GetAllOrdersByCustomerAsync(int page, int pageSize, string statusFilter = null, string categoryFilter = null,
            DateTime? startDateFilter = null,
            DateTime? endDateFilter = null,
            string productFilter = null)
        {
            var query = _context.Orders
             .Include(o => o.Customer)
              .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.Seller)
        .AsQueryable();

            // 2. إضافة فلترة لو موجودة
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(x => x.Status.ToString() == statusFilter);
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(x => x.Status.ToString() == statusFilter);
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(order => order.OrderItems
     .Any(oi => oi.Product.Category.Name.Contains(categoryFilter)));
            }

            if (startDateFilter.HasValue)
            {
                query = query.Where(x => x.OrderDate >= startDateFilter.Value);
            }
            if (endDateFilter.HasValue)
            {
                query = query.Where(x => x.OrderDate <= endDateFilter.Value);
            }

            if (!string.IsNullOrEmpty(productFilter))
            {
                query = query.Where(order => order.OrderItems
            .Any(oi => oi.Product.Category.Name.Contains(productFilter)));
     
            }
            var totalCount = await query.CountAsync();
            var ordersData = await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new StaticPagedList<Order>(ordersData, page, pageSize, totalCount);
        }

    
    }
}
