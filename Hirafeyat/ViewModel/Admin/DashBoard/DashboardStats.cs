namespace Hirafeyat.ViewModel.Admin.DashBoard
{
    public class DashboardStats
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }

        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }

        public int ApprovedProducts { get; set; }
        public int PendingProducts { get; set; }
        public int RejectedProducts { get; set; }
        public int LowStockProducts { get; set; }

        public int PaidPayments { get; set; }
        public int PendingPayments { get; set; }
        public int FailedPayments { get; set; }

        public int ActiveSellers { get; set; }
    }
}
