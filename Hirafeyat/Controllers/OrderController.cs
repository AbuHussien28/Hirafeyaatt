using Hirafeyat.AdminServices;
using Hirafeyat.CustomerRepository;
using Hirafeyat.Models;
using Hirafeyat.ViewModel;
using Hirafeyat.ViewModel.OrderCustomer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
[CustomerAuthorize]
public class OrderController : Controller
{
    private readonly HirafeyatContext _context;
    private readonly IOrderCustomerService orderService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration configuration;
    private readonly ILogger<OrderController> logger;

    public OrderController(IOrderCustomerService orderService, UserManager<ApplicationUser> userManager , IConfiguration configuration,ILogger<OrderController> logger)
    {
        this.orderService = orderService;
        _userManager = userManager;
        this.configuration = configuration;
        this.logger = logger;
        var stripeSecretKey = configuration["Stripe:SecretKey"];
        if (!string.IsNullOrEmpty(stripeSecretKey))
        {
            StripeConfiguration.ApiKey = stripeSecretKey;
        }
        else
        {
            throw new Exception("Stripe Secret Key is not configured");
        }
    }
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = await orderService.PrepareCheckoutAsync(userId);

            if (string.IsNullOrEmpty(model.StripePublishableKey))
            {
                throw new Exception("Payment system configuration error");
            }

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Index", "Cart");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(
        CheckoutPageViewModel model,
        string paymentMethod,
        string paymentIntentId = null)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orderId = await orderService.ProcessOrderAsync(
                userId, model, paymentMethod, paymentIntentId);

            logger.LogInformation("Order {OrderId} placed successfully by user {UserId}", orderId, userId);

            return RedirectToAction("PaymentSuccess", "Payment", new { orderId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error placing order for user {UserId}", _userManager.GetUserId(User));
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("PaymentFailed", "Payment");
        }
    }
    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            var orders = await orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Error loading orders: " + ex.Message;
            return View(Enumerable.Empty<OrderWithPaymentViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            var success = await orderService.CancelOrderAsync(id, userId);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to cancel order";
            }

            return RedirectToAction("MyOrders");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("MyOrders");
        }
    }
}
