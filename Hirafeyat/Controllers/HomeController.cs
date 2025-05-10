using System.Diagnostics;
using Hirafeyat.Models;
using Hirafeyat.SellerServices;
using Hirafeyat.Services;
using Hirafeyat.ViewModel;
using Hirafeyat.ViewModel.Role;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderService orderService;
        private readonly ILogger<HomeController> _logger;
       // private readonly HirafeyatContext con;

        public HomeController(ICategoryRepository categoryRepository, IProductRepository productRepository, IOrderService orderService, ILogger<HomeController> logger)
        {
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
            this.orderService = orderService;
            _logger = logger;
           
        }

        public IActionResult Index()
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

        [HttpGet]
        public IActionResult FilteredProducts(string searchText, List<string> selectedPrices)
        {
            var products = productRepository.getAll();  // جلب جميع المنتجات باستخدام الريبوستوري

            // التحقق من وجود فلاتر للأسعار
            if (selectedPrices != null && selectedPrices.Count > 0)
            {
                var filteredProducts = new List<Product>();

                // إذا كانت "all" مختارة، عرض جميع المنتجات
                if (selectedPrices.Contains("all"))
                {
                    return PartialView("_ProductListPartial", products);  // عرض جميع المنتجات بدون فلترة
                }

                // تطبيق الفلاتر حسب الفئات السعرية المحددة
                foreach (var range in selectedPrices)
                {
                    var parts = range.Split('-');
                    if (parts.Length == 2 &&
                        decimal.TryParse(parts[0], out var minPrice) &&
                        decimal.TryParse(parts[1], out var maxPrice))
                    {
                        var matching = products
                            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                            .ToList();
                        filteredProducts.AddRange(matching);
                    }
                }

                // إزالة التكرار بعد إضافة المنتجات
                products = filteredProducts.Distinct().ToList();
            }

            // التصفية حسب النص البحثي إذا كان موجودًا
            if (!string.IsNullOrEmpty(searchText))
            {
                products = products.Where(p => p.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // إرجاع المنتجات بعد الفلترة إلى العرض الجزئي
            return PartialView("_ProductListPartial", products);
        }


        public IActionResult Filter(string searchQuery, List<string> prices)
        {
            var products = productRepository.getAll();  // جلب جميع المنتجات باستخدام الريبوستوري

            // فلترة المنتجات حسب النص البحثي
            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // فلترة المنتجات حسب الأسعار المحددة
            if (prices != null && prices.Any())
            {
                foreach (var range in prices)
                {
                    var parts = range.Split('-');
                    if (parts.Length == 2 &&
                        decimal.TryParse(parts[0], out var minPrice) &&
                        decimal.TryParse(parts[1], out var maxPrice))
                    {
                        products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
                    }
                }
            }

            // إرجاع المنتجات المصفاة بصيغة JSON
            return Json(products);
        }


        public IActionResult cart()
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

        //public IActionResult Index2()
        //{
        //    var categories = categoryRepository.getAll();
        //    var products = productRepository.getAll();

        //    var groupedProducts = new Dictionary<string, List<Product>>();

        //    foreach (var category in categories)
        //    {
        //        var productsInCategory = products
        //            .Where(p => p.CategoryId == category.Id).ToList();

        //        groupedProducts[category.Name] = productsInCategory;
        //    }

        //    var vm = new homeviewmodel()
        //    {
        //        Categories = categories,
        //        Products = products,
        //        ProductDictionary = groupedProducts
        //    };

        //    return View(vm);
        //}



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
