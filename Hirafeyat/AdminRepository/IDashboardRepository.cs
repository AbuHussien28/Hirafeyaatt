namespace Hirafeyat.AdminRepository
{
    public interface IDashboardRepository
    {
        // إحصائيات عامة
        Task<int> GetTotalProductsAsync();
        Task<int> GetTotalOrdersAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalCategoriesAsync();

        // إحصائيات الطلبات
        Task<int> GetPendingOrdersCountAsync();
        Task<int> GetProcessingOrdersCountAsync();
        Task<int> GetDeliveredOrdersCountAsync();
        Task<int> GetCancelledOrdersCountAsync();

        // إحصائيات المنتجات
        Task<int> GetApprovedProductsCountAsync();
        Task<int> GetPendingProductsCountAsync();
        Task<int> GetRejectedProductsCountAsync();
        Task<int> GetLowStockProductsCountAsync(int threshold = 10);

        // إحصائيات الدفع
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetMonthlyRevenueAsync();
        Task<int> GetPaidPaymentsCountAsync();
        Task<int> GetPendingPaymentsCountAsync();
        Task<int> GetFailedPaymentsCountAsync();

        // آخر البيانات
        Task<List<Order>> GetRecentOrdersAsync(int count = 5);
        Task<List<Product>> GetRecentProductsAsync(int count = 5);
        Task<List<ApplicationUser>> GetRecentUsersAsync(int count = 5);
        Task<decimal> GetPreviousMonthRevenueAsync();
        Task<int> GetCurrentMonthOrdersCountAsync();
        Task<int> GetPreviousMonthOrdersCountAsync();
        // إحصائيات حسب الفترة الزمنية
        Task<Dictionary<string, int>> GetOrdersByMonthAsync(int months = 6);
        Task<Dictionary<string, decimal>> GetRevenueByMonthAsync(int months = 6);
        Task<List<Product>> GetTopSellingProductsAsync(int count = 5);

        // إحصائيات البائعين
        Task<List<ApplicationUser>> GetTopSellersAsync(int count = 5);
        Task<int> GetActiveSellersCountAsync();
    }
}
