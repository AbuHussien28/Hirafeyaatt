namespace Hirafeyat.ViewModel.Admin.DashBoard
{
    public class DashboardData
    {
        public DashboardStats Stats { get; set; }
        public List<Order> RecentOrders { get; set; }
        public List<Product> RecentProducts { get; set; }
        public List<ApplicationUser> RecentUsers { get; set; }
        public List<Product> TopSellingProducts { get; set; }
        public List<ApplicationUser> TopSellers { get; set; }
        public Dictionary<string, int> OrdersByMonth { get; set; }
        public Dictionary<string, decimal> RevenueByMonth { get; set; }
    }
}
