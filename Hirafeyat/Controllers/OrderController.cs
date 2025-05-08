using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hirafeyat.Models;
using System.Linq;
using System.Threading.Tasks;
using Hirafeyat.ViewModel;

public class OrderController : Controller
{
    private readonly HirafeyatContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(HirafeyatContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        ViewBag.CartItems = cartItems;
        ViewBag.TotalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);

        return View(new CheckoutViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
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
            Status = OrderStatus.Pending,
            OrderItems = cartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("OrderConfirmation");
    }


    public IActionResult OrderConfirmation()
    {
        return View();
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
