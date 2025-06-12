using Hirafeyat.ViewModel.Admin.DashBoard;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers.Admin
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = await dashboardService.GetFullDashboardDataAsync();
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.ErrorMessage = "حدث خطأ أثناء تحميل البيانات";
                return View(new DashboardData());
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetDashboardStats()
        {
            try
            {
                var stats = await dashboardService.GetDashboardStatsAsync();
                return Json(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOrdersChartData(int months = 6)
        {
            try
            {
                var data = await dashboardService.GetOrdersChartDataAsync(months);
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRevenueChartData(int months = 6)
        {
            try
            {
                var data = await dashboardService.GetRevenueChartDataAsync(months);
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRecentOrders(int count = 5)
        {
            try
            {
                var orders = await dashboardService.GetRecentOrdersAsync(count);
                var result = orders.Select(o => new
                {
                    id = o.Id,
                    customerName = o.Customer?.FullName ?? o.FullName,
                    amount = o.Payment?.Amount ?? 0,
                    status = o.Status.ToString(),
                    orderDate = o.OrderDate.ToString("dd MMM yyyy"),
                    paymentStatus = o.Payment?.Status.ToString() ?? "غير محدد"
                });

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetLowStockProducts(int threshold = 10)
        {
            try
            {
                var products = await dashboardService.GetLowStockProductsAsync(threshold);
                var result = products.Select(p => new
                {
                    id = p.Id,
                    title = p.Title,
                    quantity = p.Quentity,
                    price = p.Price,
                    category = p.Category?.Name ?? "غير محدد",
                    seller = p.Seller?.FullName ?? "غير محدد"
                });

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTopSellingProducts(int count = 5)
        {
            try
            {
                var products = await dashboardService.GetTopSellingProductsAsync(count);
                var result = products.Select(p => new
                {
                    id = p.Id,
                    title = p.Title,
                    price = p.Price,
                    ordersCount = p.Orders?.Count ?? 0,
                    category = p.Category?.Name ?? "غير محدد",
                    seller = p.Seller?.FullName ?? "غير محدد"
                });

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Analytics()
        {
            try
            {
                var stats = await dashboardService.GetDashboardStatsAsync();
                var ordersData = await dashboardService.GetOrdersChartDataAsync(12);
                var revenueData = await dashboardService.GetRevenueChartDataAsync(12);

                ViewBag.Stats = stats;
                ViewBag.OrdersData = ordersData;
                ViewBag.RevenueData = revenueData;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "حدث خطأ أثناء تحميل البيانات التحليلية";
                return View();
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetRevenueGrowthPercentage()
        {
            try
            {
                var growthPercentage = await dashboardService.GetRevenueGrowthPercentageAsync();
                return Json(new { success = true, percentage = growthPercentage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, percentage = 0 });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderGrowthPercentage()
        {
            try
            {
                var growthPercentage = await dashboardService.GetOrderGrowthPercentageAsync();
                return Json(new { success = true, percentage = growthPercentage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, percentage = 0 });
            }
        }
    }
}
