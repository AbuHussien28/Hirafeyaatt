using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using Hirafeyat.AdminServices;
using Hirafeyat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EntityFramework;
using System.Threading.Tasks;
using System.Data.Entity;
using Hirafeyat.ViewModel.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.IdentityModel.Tokens;
using Hirafeyat.ViewModel;
namespace Hirafeyat.Controllers.Admin
{
    //[Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public ProductController(
            IProductService productService,
            UserManager<ApplicationUser> userManager,
            ICategoryService categoryService)
        {
            _productService = productService;
            _userManager = userManager;
           // _mapper = mapper;
            _categoryService = categoryService;
        }

        [Route("/Admin/Product/Index")]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? sellerId = null)
        {
            var products = await _productService.GetProductsAsync(pageNumber, pageSize, sellerId);

            if (products == null || !products.Any())
            {
                ViewBag.Message = "No products available.";
            }

            var totalProducts = await _productService.GetTotalProductsCountAsync(sellerId);

            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SellerId = sellerId;
            return View("~/Views/AdminProducts/Index.cshtml", products);
        }
        
        [HttpGet]
        [Route("/Admin/Product/AjaxSearch")]
        public async Task<IActionResult> AjaxSearch([FromQuery]string name, int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productService.GetProductsByNameAsync(pageNumber,pageSize,name);

            ViewBag.Name = name;

            return PartialView("~/Views/AdminProducts/_SearchResultsPartial.cshtml", products);
        }

        [Route("/Admin/Product/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("/Views/AdminProducts/Details.cshtml", product);
        }
       
        
        [Route("/Admin/Product/DeleteAsync/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("/Views/AdminProducts/Delete.cshtml", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Route("/Admin/Product/EditStatus/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditStatus(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductStatusViewModel
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                Title = product.Title,
                Status = product.Status
            };

            return View("/Views/AdminProducts/Edit.cshtml", viewModel);
        }

        [Route("/Admin/Product/EditStatus/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(ProductStatusViewModel model , int id)
        {
            if (!ModelState.IsValid)
            {
                return View("/Views/AdminProducts/Edit.cshtml", model);
            }

            var product = await _productService.GetProductByIdAsync(model.Id);
            if (product == null)
            {
                return NotFound();
            }
            product.Status = model.Status.Value;
            await _productService.UpdateProductAsync(product);

            return RedirectToAction(nameof(Index));
        }

        #region Search

        [HttpGet]
        [Route("/Admin/Product/Board")]
        public async Task<IActionResult> Board([FromQuery]string name, int pageNumber = 1, int pageSize = 10)
        {

            return PartialView("~/Views/AdminProducts/DashBoard.cshtml");
        }
        #endregion
    }
}
