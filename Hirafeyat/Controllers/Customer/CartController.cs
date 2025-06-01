using Hirafeyat.Models;
using Hirafeyat.ViewModel.NewFolder;
     
using Hirafeyat.ViewModel.NewFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.Controllers.Customer
{

    public class CartController : Controller
    {
        private readonly HirafeyatContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(HirafeyatContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var existingItem = _context.CartItems.FirstOrDefault(c => c.ProductId == productId && c.UserId == user.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    _context.CartItems.Add(new CartItem { ProductId = productId, UserId = user.Id, Quantity = 1 });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("MyCart");
            }
            else
            {
                // Guest cart in Session
                var cartString = HttpContext.Session.GetString("cart");
                List<CartItemSession> cartItems = string.IsNullOrEmpty(cartString)
                    ? new List<CartItemSession>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<CartItemSession>>(cartString);

                var existingItem = cartItems.FirstOrDefault(c => c.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cartItems.Add(new CartItemSession { ProductId = productId, Quantity = 1 });
                }

                HttpContext.Session.SetString("cart", System.Text.Json.JsonSerializer.Serialize(cartItems));

                return RedirectToAction("GuestCart");
            }
        }
        public class CartItemSession
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }


        public async Task<IActionResult> MyCart()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                var cartItems = await _context.CartItems
                                     .Where(c => c.UserId == user.Id)
                                     .Include(c => c.Product)
                                     .ToListAsync();

                var cartVM = new cartvm
                {
                    Items = cartItems.Select(c => new CartItemViewModel
                    {
                        ProductId = c.ProductId,
                        ProductName = c.Product.Title,
                        ImageUrl = c.Product.ImageUrl,
                        Price = c.Product.Price,
                        Quantity = c.Quantity
                    }).ToList()
                };

                return View("MyCart", cartVM);
            }
            else
            {
                var cartString = HttpContext.Session.GetString("cart");
                List<CartItemSession> cartItems = string.IsNullOrEmpty(cartString)
                    ? new List<CartItemSession>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<CartItemSession>>(cartString);

                var productIds = cartItems.Select(c => c.ProductId).ToList();

                var products = _context.Products
                                .Where(p => productIds.Contains(p.Id))
                                .ToList();

                var cartVM = new cartvm
                {
                    Items = cartItems.Select(ci =>
                    {
                        var product = products.FirstOrDefault(p => p.Id == ci.ProductId);
                        return new CartItemViewModel
                        {
                            ProductId = ci.ProductId,
                            ProductName = product?.Title ?? "Unknown",
                            ImageUrl = product?.ImageUrl ?? "",
                            Price = product?.Price ?? 0,
                            Quantity = ci.Quantity
                        };
                    }).ToList()
                };

                return View("GuestCart", cartVM);
            }
        }

        public async Task<IActionResult> GuestCart()
        {
            return await MyCart();
        }





        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var item = await _context.CartItems.FirstOrDefaultAsync(f => f.ProductId == id);
                if (item != null)
                {
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("MyCart");
            }
            else
            {

                var cart = HttpContext.Session.GetString("CartItems");
                List<int> cartList = string.IsNullOrEmpty(cart) ? new List<int>() : System.Text.Json.JsonSerializer.Deserialize<List<int>>(cart);

                if (cartList.Contains(id))
                {
                    cartList.Remove(id);
                    HttpContext.Session.SetString("CartItems", System.Text.Json.JsonSerializer.Serialize(cartList));
                }

                return RedirectToAction("GuestCart");
            }
        }


[HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                // المستخدم مسجل دخول
                var userId = _userManager.GetUserId(User);
                var item = await _context.CartItems
                                         .FirstOrDefaultAsync(c => c.ProductId == id && c.UserId == userId);

                if (item != null)
                {
                    item.Quantity = quantity < 1 ? 1 : quantity;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // المستخدم ضيف (جلسة)
                var cartString = HttpContext.Session.GetString("cart");
                var cartItems = string.IsNullOrEmpty(cartString)
                    ? new List<CartItemSession>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<CartItemSession>>(cartString);

                var cartItem = cartItems.FirstOrDefault(c => c.ProductId == id);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity < 1 ? 1 : quantity;
                    HttpContext.Session.SetString("cart", System.Text.Json.JsonSerializer.Serialize(cartItems));
                }
            }

            return RedirectToAction("MyCart");
        }


    }
}
