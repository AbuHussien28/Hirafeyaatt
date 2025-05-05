using System.Diagnostics;
using Hirafeyat.Models;
using Hirafeyat.ViewModel;
using Hirafeyat.ViewModel.Role;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HirafeyatContext con;

        public HomeController(ILogger<HomeController> logger, HirafeyatContext con)
        {
            _logger = logger;
            this.con = con;
        }

        public IActionResult Index()
        {
            var categorize = con.Categories.ToList();
            var products = con.Products.ToList();
            var vm = new homeviewmodel()
            {
                Products = products,
                Categories = categorize
            };
            return View(vm);
        }

        public IActionResult contact()
        {

            var categorize = con.Categories.ToList();
            var products = con.Products.ToList();
            var vm = new homeviewmodel()
            {
                Products = products,
                Categories = categorize
            };
            return View(vm);
        }

        public IActionResult shop()
        {

            var categorize = con.Categories.ToList();
            var products = con.Products.ToList();
            var vm = new homeviewmodel()
            {
                Products = products,
                Categories = categorize
            };
            return View(vm);


        }
        public IActionResult cart()
        {
            var categorize = con.Categories.ToList();
            var products = con.Products.ToList();
            var vm = new homeviewmodel()
            {
                Products = products,
                Categories = categorize
            };
            return View(vm);
        }

        public IActionResult checkout()
        {
            var categorize = con.Categories.ToList();
            var products = con.Products.ToList();
            var vm = new homeviewmodel()
            {
                Products = products,
                Categories = categorize
            };
            return View(vm);
        }
        public IActionResult info()
        {
            var categorize = con.Categories.ToList();
            var products = con.Products.ToList();
            var vm = new homeviewmodel()
            {
                Products = products,
                Categories = categorize
            };
            return View(vm);
        }

        public IActionResult checkoutpartialview()
        {
            return View();
        }

        public IActionResult producthomepatialview()
        {
            var products = con.Products.ToList();

            return View("producthomepatialview", products);

        }


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
