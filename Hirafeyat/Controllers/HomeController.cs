using Hirafeyat.Models;
using Hirafeyat.SellerServices;
using Hirafeyat.Services;
using Hirafeyat.ViewModel;
using Hirafeyat.ViewModel.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hirafeyat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderService orderService;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ICategoryRepository categoryRepository, IProductRepository productRepository, IOrderService orderService, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
            this.orderService = orderService;
            _logger = logger;
            this.userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(User);
                var roles = await userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Dashboard");

                if (roles.Contains("Seller"))
                    return RedirectToAction("Index", "Seller");
                 
                    
            }
            var categories = categoryRepository.getAll();
            var products = productRepository.getAll();

            var groupedProducts = new Dictionary<string, List<Product>>();

            foreach (var category in categories)
            {
                var productsInCategory = products
                    .Where(p => p.CategoryId == category.Id).ToList();

                groupedProducts[category.Name] = productsInCategory;
            }

            var vm = new homeviewmodel()
            {
                Categories = categories,
                Products = products,
                ProductDictionary = groupedProducts
            };

            return View(vm);
        
        }

        public IActionResult contact()
        {

            var categories = categoryRepository.getAll();
            var products = productRepository.getAll();

            var groupedProducts = new Dictionary<string, List<Product>>();

            foreach (var category in categories)
            {
                var productsInCategory = products
                    .Where(p => p.CategoryId == category.Id).ToList();

                groupedProducts[category.Name] = productsInCategory;
            }

            var vm = new homeviewmodel()
            {
                Categories = categories,
                Products = products,
                ProductDictionary = groupedProducts
            };

            return View(vm);
        }


        public IActionResult Shop()
        {
            // جلب جميع المنتجات من المستودع عند تحميل الصفحة
            var products = productRepository.getAll();
            return View(products); // إرجاع المنتجات إلى الـ View
        }

        
        [HttpPost]
        public JsonResult FilterProducts(List<string> SelectedPrices)
        {
            var products = productRepository.getAll();

            if (SelectedPrices != null && SelectedPrices.Count > 0)
            {
                // فلترة المنتجات بناءً على الأسعار المحددة
                if (!SelectedPrices.Contains("all"))
                {
                    products = products.Where(p =>
                        (SelectedPrices.Contains("0-100") && p.Price >= 0 && p.Price <= 100) ||
                        (SelectedPrices.Contains("100-200") && p.Price > 100 && p.Price <= 200) ||
                        (SelectedPrices.Contains("200-300") && p.Price > 200 && p.Price <= 300)
                    ).ToList();
                }
            }

            // إعادة القائمة المصفاة كـ JSON
            var filteredProducts = products.Select(p => new
            {
                id = p.Id,
                title = p.Title,
                price = p.Price,
                imageUrl = p.ImageUrl,
                category = new { name = p.Category.Name }
            });

            return Json(filteredProducts);
        }
        [HttpGet]
        public JsonResult SearchProducts(string query)
        {
            var products = productRepository.getAll();

            if (!string.IsNullOrEmpty(query))
            {
                products = products.Where(p => p.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var filteredProducts = products.Select(p => new
            {
                id = p.Id,
                title = p.Title,
                price = p.Price,
                imageUrl = p.ImageUrl,
                category = new { name = p.Category.Name }
            });

            return Json(filteredProducts);
        }

        [HttpGet]
        public JsonResult GetAllProducts()
        {
            var products = productRepository.getAll();
            var allProducts = products.Select(p => new
            {
                id = p.Id,
                title = p.Title,
                price = p.Price,
                imageUrl = p.ImageUrl,
                category = new { name = p.Category.Name }
            });

            return Json(allProducts);
        }
         public IActionResult Details(int id)
        {
            Product pro = productRepository.getById(id);
            var products = productRepository.getAll();
            edithomeviewmodel vm = new edithomeviewmodel()
            {
                products = products,
                product = pro
            };
            if(vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        public IActionResult checkout()
        {
            var categories = categoryRepository.getAll();
            var products = productRepository.getAll();

            var groupedProducts = new Dictionary<string, List<Product>>();

            foreach (var category in categories)
            {
                var productsInCategory = products
                    .Where(p => p.CategoryId == category.Id).ToList();

                groupedProducts[category.Name] = productsInCategory;
            }

            var vm = new homeviewmodel()
            {
                Categories = categories,
                Products = products,
                ProductDictionary = groupedProducts
            };

            return View(vm);
        }
        public IActionResult info()
        {
            var categories = categoryRepository.getAll();
            var products = productRepository.getAll();

            var groupedProducts = new Dictionary<string, List<Product>>();

            foreach (var category in categories)
            {
                var productsInCategory = products
                    .Where(p => p.CategoryId == category.Id).ToList();

                groupedProducts[category.Name] = productsInCategory;
            }

            var vm = new homeviewmodel()
            {
                Categories = categories,
                Products = products,
                ProductDictionary = groupedProducts
            };

            return View(vm);
        }

        public IActionResult checkoutpartialview()
        {
            return View();
        }

        public IActionResult producthomepatialview()
        {
            var products = productRepository.getAll();

            return View("producthomepatialview", products);

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
