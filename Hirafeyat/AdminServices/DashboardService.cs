using Hirafeyat.AdminRepository;
using Hirafeyat.ViewModel.Admin.DashBoard;

namespace Hirafeyat.AdminServices
{
    public class DashboardService:IDashboardService
    {
        private readonly IDashboardRepository dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            this.dashboardRepository = dashboardRepository;
        }
        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            return new DashboardStats
            {
                TotalProducts = await dashboardRepository.GetTotalProductsAsync(),
                TotalOrders = await dashboardRepository.GetTotalOrdersAsync(),
                TotalUsers = await dashboardRepository.GetTotalUsersAsync(),
                TotalCategories = await dashboardRepository.GetTotalCategoriesAsync(),
                TotalRevenue = await dashboardRepository.GetTotalRevenueAsync(),
                MonthlyRevenue = await dashboardRepository.GetMonthlyRevenueAsync(),

                PendingOrders = await dashboardRepository.GetPendingOrdersCountAsync(),
                ProcessingOrders = await dashboardRepository.GetProcessingOrdersCountAsync(),
                DeliveredOrders = await dashboardRepository.GetDeliveredOrdersCountAsync(),
                CancelledOrders = await dashboardRepository.GetCancelledOrdersCountAsync(),

                ApprovedProducts = await dashboardRepository.GetApprovedProductsCountAsync(),
                PendingProducts = await dashboardRepository.GetPendingProductsCountAsync(),
                RejectedProducts = await dashboardRepository.GetRejectedProductsCountAsync(),
                LowStockProducts = await dashboardRepository.GetLowStockProductsCountAsync(),

                PaidPayments = await dashboardRepository.GetPaidPaymentsCountAsync(),
                PendingPayments = await dashboardRepository.GetPendingPaymentsCountAsync(),
                FailedPayments = await dashboardRepository.GetFailedPaymentsCountAsync(),

                ActiveSellers = await dashboardRepository.GetActiveSellersCountAsync()
            };
        }

        public async Task<DashboardData> GetFullDashboardDataAsync()
        {
            return new DashboardData
            {
                Stats = await GetDashboardStatsAsync(),
                RecentOrders = await dashboardRepository.GetRecentOrdersAsync(),
                RecentProducts = await dashboardRepository.GetRecentProductsAsync(),
                RecentUsers = await dashboardRepository.GetRecentUsersAsync(),
                TopSellingProducts = await dashboardRepository.GetTopSellingProductsAsync(),
                TopSellers = await dashboardRepository.GetTopSellersAsync(),
                OrdersByMonth = await dashboardRepository.GetOrdersByMonthAsync(),
                RevenueByMonth = await dashboardRepository.GetRevenueByMonthAsync()
            };
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count = 5)
        {
            return await dashboardRepository.GetRecentOrdersAsync(count);
        }

        public async Task<List<Product>> GetRecentProductsAsync(int count = 5)
        {
            return await dashboardRepository.GetRecentProductsAsync(count);
        }

        public async Task<List<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            // Get all products and filter by low stock
            var allProducts = await dashboardRepository.GetRecentProductsAsync(1000);
            return allProducts.Where(p => p.Quentity <= threshold).ToList();
        }

        public async Task<Dictionary<string, int>> GetOrdersChartDataAsync(int months = 6)
        {
            return await dashboardRepository.GetOrdersByMonthAsync(months);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueChartDataAsync(int months = 6)
        {
            return await dashboardRepository.GetRevenueByMonthAsync(months);
        }

        public async Task<List<Product>> GetTopSellingProductsAsync(int count = 5)
        {
            return await dashboardRepository.GetTopSellingProductsAsync(count);
        }

        public async Task<decimal> GetRevenueGrowthPercentageAsync()
        {
            var currentMonthRevenue = await dashboardRepository.GetMonthlyRevenueAsync();
            var previousMonthRevenue = await dashboardRepository.GetPreviousMonthRevenueAsync();

            if (previousMonthRevenue == 0) return 0;

            return ((currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue) * 100;
        }

        public async Task<decimal> GetOrderGrowthPercentageAsync()
        {
            var currentMonthOrders = await dashboardRepository.GetCurrentMonthOrdersCountAsync();
            var previousMonthOrders = await dashboardRepository.GetPreviousMonthOrdersCountAsync();

            if (previousMonthOrders == 0) return 0;

            return ((decimal)(currentMonthOrders - previousMonthOrders) / previousMonthOrders) * 100;
        }
    }
}
