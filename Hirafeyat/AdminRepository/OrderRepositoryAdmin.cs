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
             .Include(o => o.Product).ThenInclude(p => p.Seller)
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

            // Apply category filter if provided
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(x => x.Product.Category.Name.Contains(categoryFilter));
            }

            // Apply date filters if provided (startDate and endDate)
            if (startDateFilter.HasValue)
            {
                query = query.Where(x => x.OrderDate >= startDateFilter.Value);
            }
            if (endDateFilter.HasValue)
            {
                query = query.Where(x => x.OrderDate <= endDateFilter.Value);
            }

            // Apply product filter if provided (based on product title or any other field)
            if (!string.IsNullOrEmpty(productFilter))
            {
                query = query.Where(x => x.Product.Title.Contains(productFilter));
            }
            // 3. حساب العدد الكلي بعد الفلترة
            var totalCount = await query.CountAsync();

            // 4. تنفيذ Skip و Take لجلب البيانات الخاصة بالصفحة المطلوبة
            var ordersData = await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 5. تجهيز StaticPagedList
            return new StaticPagedList<Order>(ordersData, page, pageSize, totalCount);
        }

    
    }
}
