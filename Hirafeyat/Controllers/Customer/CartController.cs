using Hirafeyat.Models;
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
            var user = await _userManager.GetUserAsync(User);
            var existingItem = _context.CartItems.FirstOrDefault(c => c.ProductId == productId && c.UserId == user.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _context.CartItems.Add(new CartItem { ProductId = productId, UserId = user.Id });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("MyCart");
        }

        public async Task<IActionResult> MyCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var items = _context.CartItems
                                .Where(c => c.UserId == user.Id)
                                .Include(c => c.Product)
                                .ToList();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyCart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var item = await _context.CartItems.FindAsync(id);
            item.Quantity = quantity;
            await _context.SaveChangesAsync();
            return RedirectToAction("MyCart");
        }

    }
}
