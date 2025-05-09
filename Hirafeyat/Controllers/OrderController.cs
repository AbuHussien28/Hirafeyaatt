using Hirafeyat.Models;
using Hirafeyat.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Linq;
using System.Threading.Tasks;

public class OrderController : Controller
{
    private readonly HirafeyatContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration configuration;

    public OrderController(HirafeyatContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        this.configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
        var paymentIntentService = new PaymentIntentService();
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(cartItems.Sum(c => c.Product.Price * c.Quantity) * 100),
            Currency = "egp",
            PaymentMethodTypes = new List<string> { "card" }
        };
        var intent = await paymentIntentService.CreateAsync(options);

        ViewBag.CartItems = cartItems;
        ViewBag.TotalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);
        ViewBag.ClientSecret = intent.ClientSecret ;
        ViewBag.StripePublishableKey = configuration["Stripe:PublishableKey"];

        return View(new CheckoutViewModel());
    }

    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model, string paymentMethod, string paymentIntentId = null)
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!ModelState.IsValid)
        {
            ViewBag.CartItems = cartItems;
            ViewBag.TotalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);
            return View("Checkout", model);
        }
        var order = new Order
        {
            CustomerId = userId,
            FullName = model.FullName,
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            Status = paymentMethod == "cod" ? OrderStatus.Processing : OrderStatus.Pending,
            OrderItems = cartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        var payment = new Payment
        {
            Order = order,
            Amount = cartItems.Sum(c => c.Product.Price * c.Quantity),
            PaymentDate = DateTime.Now,
            PaymentMethod = paymentMethod,
            Status = paymentMethod == "cod" ? PaymentStatus.Paid : PaymentStatus.Pending
        };

        if (paymentMethod == "stripe" && !string.IsNullOrEmpty(paymentIntentId))
        {
            payment.StripePaymentIntentId = paymentIntentId ;
            var paymentIntentService = new PaymentIntentService();
            var intent = await paymentIntentService.GetAsync(paymentIntentId);

            if (intent.Status == "succeeded")
            {
                payment.Status = PaymentStatus.Paid;
                order.Status = OrderStatus.Processing;
            }
        }

        _context.Payments.Add(payment);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("PaymentSuccess", "Payment", new { orderId = order.Id });
    }

    public IActionResult OrderConfirmation()
    {
        return RedirectToAction("Checkout", "Payment");
    }
    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {
        var userId = _userManager.GetUserId(User);
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == userId && o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == userId);

        if (order == null || order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
        {
            return BadRequest("You can't cancel this order.");
        }

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();

        return RedirectToAction("MyOrders");
    }

}
