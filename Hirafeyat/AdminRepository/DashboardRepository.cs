
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.AdminRepository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly HirafeyatContext context;

        public DashboardRepository(HirafeyatContext context)
        {
            this.context = context;
        }
        public async Task<int> GetTotalProductsAsync()
        {
            return await context.Products.CountAsync();
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await context.Orders.CountAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await context.Users.CountAsync();
        }

        public async Task<int> GetTotalCategoriesAsync()
        {
            return await context.Categories.CountAsync();
        }

        // إحصائيات الطلبات
        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        }

        public async Task<int> GetProcessingOrdersCountAsync()
        {
            return await context.Orders.CountAsync(o => o.Status == OrderStatus.Processing);
        }

        public async Task<int> GetDeliveredOrdersCountAsync()
        {
            return await context.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
        }

        public async Task<int> GetCancelledOrdersCountAsync()
        {
            return await context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);
        }

        // إحصائيات المنتجات
        public async Task<int> GetApprovedProductsCountAsync()
        {
            return await context.Products.CountAsync(p => p.Status == productStatus.Approved);
        }

        public async Task<int> GetPendingProductsCountAsync()
        {
            return await context.Products.CountAsync(p => p.Status == productStatus.Pending);
        }

        public async Task<int> GetRejectedProductsCountAsync()
        {
            return await context.Products.CountAsync(p => p.Status == productStatus.Rejected);
        }

        public async Task<int> GetLowStockProductsCountAsync(int threshold = 10)
        {
            return await context.Products.CountAsync(p => p.Quentity <= threshold);
        }

        // إحصائيات الدفع - Fixed
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await context.Payments
                .Where(p => p.Status == PaymentStatus.Paid)
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await context.Payments
                .Where(p => p.Status == PaymentStatus.Paid &&
                           p.PaymentDate >= startOfMonth &&
                           p.PaymentDate <= endOfMonth)
                .SumAsync(p => p.Amount);
        }

        // إضافة method جديدة للحصول على revenue للشهر السابق
        public async Task<decimal> GetPreviousMonthRevenueAsync()
        {
            var now = DateTime.Now;
            var startOfPreviousMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
            var endOfPreviousMonth = startOfPreviousMonth.AddMonths(1).AddDays(-1);

            return await context.Payments
                .Where(p => p.Status == PaymentStatus.Paid &&
                           p.PaymentDate >= startOfPreviousMonth &&
                           p.PaymentDate <= endOfPreviousMonth)
                .SumAsync(p => p.Amount);
        }

        // إضافة methods جديدة للحصول على عدد الطلبات الشهرية
        public async Task<int> GetCurrentMonthOrdersCountAsync()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return await context.Orders
                .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                .CountAsync();
        }

        public async Task<int> GetPreviousMonthOrdersCountAsync()
        {
            var now = DateTime.Now;
            var startOfPreviousMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
            var endOfPreviousMonth = startOfPreviousMonth.AddMonths(1).AddDays(-1);

            return await context.Orders
                .Where(o => o.OrderDate >= startOfPreviousMonth && o.OrderDate <= endOfPreviousMonth)
                .CountAsync();
        }

        public async Task<int> GetPaidPaymentsCountAsync()
        {
            return await context.Payments.CountAsync(p => p.Status == PaymentStatus.Paid);
        }

        public async Task<int> GetPendingPaymentsCountAsync()
        {
            return await context.Payments.CountAsync(p => p.Status == PaymentStatus.Pending);
        }

        public async Task<int> GetFailedPaymentsCountAsync()
        {
            return await context.Payments.CountAsync(p => p.Status == PaymentStatus.Failed);
        }

        // آخر البيانات
        public async Task<List<Order>> GetRecentOrdersAsync(int count = 5)
        {
            return await context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetRecentProductsAsync(int count = 5)
        {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetRecentUsersAsync(int count = 5)
        {
            return await context.Users
                .OrderByDescending(u => u.AccountCreatedDate)
                .Take(count)
                .ToListAsync();
        }

        // إحصائيات حسب الفترة الزمنية
        public async Task<Dictionary<string, int>> GetOrdersByMonthAsync(int months = 6)
        {
            var startDate = DateTime.Now.AddMonths(-months);

            var orders = await context.Orders
                .Where(o => o.OrderDate >= startDate)
                .Select(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .ToListAsync();

            var grouped = orders
                .GroupBy(o => new { o.Year, o.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToDictionary(x => x.Date.ToString("MMM yyyy"), x => x.Count);

            return grouped;
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByMonthAsync(int months = 6)
        {
            var startDate = DateTime.Now.AddMonths(-months);

            var payments = await context.Payments
                .Where(p => p.Status == PaymentStatus.Paid && p.PaymentDate >= startDate)
                .Select(p => new { p.PaymentDate, p.Amount })
                .ToListAsync();

            var grouped = payments
                .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount = g.Sum(p => p.Amount)
                })
                .OrderBy(x => x.Date)
                .ToDictionary(x => x.Date.ToString("MMM yyyy"), x => x.Amount);

            return grouped;
        }

        public async Task<List<Product>> GetTopSellingProductsAsync(int count = 5)
        {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Orders)
                .OrderByDescending(p => p.Orders.Count)
                .Take(count)
                .ToListAsync();
        }

        // إحصائيات البائعين
        public async Task<List<ApplicationUser>> GetTopSellersAsync(int count = 5)
        {
            return await context.Users
                .Include(u => u.Products)
                .Where(u => u.Products.Any())
                .OrderByDescending(u => u.Products.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetActiveSellersCountAsync()
        {
            return await context.Users
                .Where(u => u.Products.Any(p => p.Status == productStatus.Approved))
                .CountAsync();
        }
    }
}
