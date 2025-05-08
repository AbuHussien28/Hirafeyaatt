using Hirafeyat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.Controllers.Customer
{
    public class FavoriteController : Controller
    {
        private readonly HirafeyatContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(HirafeyatContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavourites(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!_context.Favorites.Any(f => f.ProductId == productId && f.UserId == user.Id))
            {
                _context.Favorites.Add(new Favorite { ProductId = productId, UserId = user.Id });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MyList");
        }

        public async Task<IActionResult> MyList()
        {
            var user = await _userManager.GetUserAsync(User);
            var favs = _context.Favorites
                               .Where(f => f.UserId == user.Id)
                               .Include(f => f.Product)
                               .ToList();
            return View(favs);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavourites(int id)
        {
            var item = await _context.Favorites.FindAsync(id);
            _context.Favorites.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyList");
        }

    }
}
