using Hirafeyat.AdminServices;
using Hirafeyat.ViewModel.Admin;
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
        [Route("/User/SellerDetails")]
        public async Task<IActionResult> SellerDetails([FromQuery]string username)
        {
            
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required");
            }
            var user = await userService.SellerDetailsAsync(username);
            return View("SellerDetails", user);
        }
        public async Task<IActionResult> CustomerDetails(string username)
        {
            
            var user = await userService.CustomerDetailsAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return View("CustomerDetails", user);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchToggleUserStatus([FromBody] List<string> userNames, bool activate)
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
        [HttpGet]
        public async Task<IActionResult> SearchSellers(string query, int page = 1)
        {
            int pageSize = 10; 
            var sellers = await userService.GetSellersAsync(page, pageSize, query);
            return PartialView("SellerTablePartial", sellers);
        }
        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string query, int page = 1)
        {
            int pageSize = 10;
            var customers = await userService.GetCustomersAsync(page, pageSize, query);
            return PartialView("CustomerTablePartial", customers);
        }

    }
}
