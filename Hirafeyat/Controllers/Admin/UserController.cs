using Hirafeyat.AdminServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hirafeyat.Controllers.Admin
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        public async Task<IActionResult> Sellers(int page = 1, int pageSize = 10)
        {
            ViewData["PaginationAction"] = "Sellers";
            var sellers = await userService.GetSellersAsync(page, pageSize); 

            return View("Sellers", sellers);
        }
        public async Task<IActionResult> Customers(int page = 1, int pageSize = 10)
        {
            ViewData["PaginationAction"] = "Customers";
            var customers = await userService.GetCustomersAsync(page, pageSize); 
            return View("Customers", customers);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userName, string sourceAction)
        {
            try
            {
                var resultFromStatus = await userService.ToggleUserStatus(userName);
                if (resultFromStatus)
                {
                    TempData["SuccessMessage"] = "User status updated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update user status";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating status: {ex.Message}";
            }
            return sourceAction switch
            {
                "Sellers" => RedirectToAction("Sellers"),
                _ => RedirectToAction("Customers") 
            };
        }
        [HttpPost]
        public async Task<IActionResult> BatchToggleUserStatus(List<string> userNames, bool activate)
        {
            try
            {
                await userService.BatchToggleUserStatusAsync(userNames, activate);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
