using Hirafeyat.ViewModel.Admin.DashBoard;

namespace Hirafeyat.AdminServices
{
    public interface IDashboardService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<DashboardData> GetFullDashboardDataAsync();
        Task<List<Order>> GetRecentOrdersAsync(int count = 5);
        Task<List<Product>> GetRecentProductsAsync(int count = 5);
        Task<List<Product>> GetLowStockProductsAsync(int threshold = 10);
        Task<Dictionary<string, int>> GetOrdersChartDataAsync(int months = 6);
        Task<Dictionary<string, decimal>> GetRevenueChartDataAsync(int months = 6);
        Task<List<Product>> GetTopSellingProductsAsync(int count = 5);
        Task<decimal> GetRevenueGrowthPercentageAsync();
        Task<decimal> GetOrderGrowthPercentageAsync();
    }
}
