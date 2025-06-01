using System.Diagnostics;
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
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                bool alreadyFavorited = await _context.Favorites
                    .AnyAsync(f => f.ProductId == productId && f.UserId == user.Id);

                if (!alreadyFavorited)
                {
                    _context.Favorites.Add(new Favorite { ProductId = productId, UserId = user.Id });
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("MyList");   // للمستخدمين المسجلين
            }
            else
            {
                // Handle Guest Favorites using Session
                var favs = HttpContext.Session.GetString("Favorites");
                List<int> favList;

                if (string.IsNullOrEmpty(favs))
                {
                    // لا يوجد بيانات في السيشن، ننشئ قائمة جديدة
                    favList = new List<int>();
                }
                else
                {
                    favList = System.Text.Json.JsonSerializer.Deserialize<List<int>>(favs);
                }

                if (!favList.Contains(productId))
                {
                    favList.Add(productId);
                    HttpContext.Session.SetString("Favorites", System.Text.Json.JsonSerializer.Serialize(favList));
                }

                return RedirectToAction("GuestList");
            }
        }



        public IActionResult GuestList()
        {
            var favs = HttpContext.Session.GetString("Favorites");
            List<int> favList = string.IsNullOrEmpty(favs) ? new List<int>() : System.Text.Json.JsonSerializer.Deserialize<List<int>>(favs);

            var products = _context.Products
                                   .Where(p => favList.Contains(p.Id))
                                   .ToList();

            return View(products); 
        }

        public async Task<IActionResult> MyList()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var favs = _context.Favorites
                                   .Where(f => f.UserId == user.Id)
                                   .Include(f => f.Product)
                                   .ToList();
                return View("MyList", favs);
            }
            else
            {
                var favs = new List<Product>();
                var favsString = HttpContext.Session.GetString("Favorites");
                if (!string.IsNullOrEmpty(favsString))
                {
                    var favIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(favsString);
                    favs = _context.Products.Where(p => favIds.Contains(p.Id)).ToList();
                }
                return View("GuestList", favs);
            }
        }



        //[Authorize(Roles = "Customer")]
[HttpPost]
public async Task<IActionResult> RemoveFromFavourites(int id)
{
    if (User.Identity.IsAuthenticated)
    {
        // حالة المستخدم المسجل: نحذف من قاعدة البيانات
        var item = await _context.Favorites.FirstOrDefaultAsync(f => f.ProductId == id);
                if (item != null)
        {
            _context.Favorites.Remove(item);
            _context.SaveChangesAsync();
        }
        return RedirectToAction("MyList");
    }
    else
    {
        // حالة الضيف: نحذف من السيشن
        var favs = HttpContext.Session.GetString("Favorites");
        List<int> favList = string.IsNullOrEmpty(favs) ? new List<int>() : System.Text.Json.JsonSerializer.Deserialize<List<int>>(favs);

        if (favList.Contains(id))
        {
            favList.Remove(id);
            HttpContext.Session.SetString("Favorites", System.Text.Json.JsonSerializer.Serialize(favList));
        }

        return RedirectToAction("GuestList");
    }
}


    }
}
