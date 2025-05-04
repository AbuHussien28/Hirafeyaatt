using Hirafeyat.AdminServices;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers.Admin
{
    public class AdminOrderController : Controller
    {
        private readonly IOrderAdminService orderAdminService;

        public AdminOrderController(IOrderAdminService orderAdminService)
        {
            this.orderAdminService = orderAdminService;
        }
        public async Task<IActionResult> Index(string categoryFilter,
    DateTime? startDateFilter ,
    DateTime? endDateFilter ,
    string productFilter,string statusFilter,int page=1,int pageSize=10)
        {
            ViewData["PaginationAction"] = "Index";
            ViewData["StatusFilter"] = statusFilter; 
            ViewData["StatusFilter"] = statusFilter;
            ViewData["CategoryFilter"] = categoryFilter;
            ViewData["ProductFilter"] = productFilter;
            ViewData["StartDateFilter"] = startDateFilter?.ToString("yyyy-MM-dd");
            ViewData["EndDateFilter"] = endDateFilter?.ToString("yyyy-MM-dd");
            ViewData["PaginationAction"] = "Index";
            var orders = await orderAdminService.GetAllOrdersByCustomerAsync(page, pageSize, statusFilter,categoryFilter,startDateFilter,endDateFilter,productFilter);
            return View(orders);
        }
    }
}
