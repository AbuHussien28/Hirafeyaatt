using Hirafeyat.Models;
using Hirafeyat.SellerServices;
using Hirafeyat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers
{
   // [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderService orderService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> user;
        public CustomerController(ICategoryRepository categoryRepository, IProductRepository productRepository, IOrderService orderService, UserManager<ApplicationUser> user)
        {
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
            this.orderService = orderService;
            this.user = user;

        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = categoryRepository.getAll();
            var viewModel = new Dictionary<string, List<Product>>();

            foreach (var category in categories)
            {
                var productsInCategory = productRepository.GetAllWithSeller()
                    .Where(p => p.CategoryId == category.Id).ToList();
                viewModel[category.Name] = productsInCategory;
            }

            return View("~/Views/Customer/Index.cshtml", viewModel);
        }

        //public IActionResult Index()
        //{
        //    List<Product> products = productRepository.GetAllWithSeller();
        //    return View("~/Views/Customer/Index.cshtml",products);
        //}
    }
}
