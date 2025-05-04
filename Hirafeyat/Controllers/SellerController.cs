using Hirafeyat.SellerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Hirafeyat.Models;
using Hirafeyat.Services;
using System.Data;
using Hirafeyat.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Hirafeyat.ViewModel.sellerVM;

namespace Hirafeyat.Controllers
{
    public class SellerController : Controller
    {
        private readonly IProductRepository ProductRepository;
        private readonly ICategoryRepository CategoryRepository;
        private readonly IOrderService orderService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> user;

        public SellerController(IOrderService orderService,UserManager<ApplicationUser> user , IProductRepository productRepo , ICategoryRepository catRepo)
        {
            this.orderService = orderService;
            this.user = user;
            ProductRepository = productRepo;
            CategoryRepository = catRepo;
        }
        public IActionResult Index()
        {
            var sId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            List<Product> products = ProductRepository.getProductsBySellerId(sId);
            return View("Index" , products);
        }
        public IActionResult New()
        {
            ViewData["CatList"] = CategoryRepository.getAll() ;
            return View("New");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewProductViewModel productfromModel)
        {
            if (ModelState.IsValid == true) {
                string imageUrl = null;
                if(productfromModel.Image != null && productfromModel.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot", "Imges");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + productfromModel.Image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productfromModel.Image.CopyToAsync(fileStream);
                    }

                    imageUrl = "/Imges/" + uniqueFileName;

                    var product = new Product
                    {
                        Title = productfromModel.Title,
                        Description = productfromModel.Description,
                        Price = productfromModel.Price,
                        Quentity = productfromModel.Quentity,
                        ImageUrl = imageUrl,
                        CategoryId = productfromModel.catId,
                        SellerId = productfromModel.sellerId,
                        CreatedAt = DateTime.Now
                    };
                    try
                    {
                        ProductRepository.add(product);
                        ProductRepository.save();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("other", ex.InnerException.Message);

                    }
                }
            }
            
            ViewData["catList"] = CategoryRepository.getAll();
            return View(productfromModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = ProductRepository.getById(id);
            if (product == null) {
                return NotFound();
            }

            var productToEdit = new NewProductViewModel
            {
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Quentity = product.Quentity,
                catId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                sellerId = product.SellerId
            };
            ViewBag.ProductId = id;
            ViewData["catList"]= CategoryRepository.getAll();
            return View(productToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewProductViewModel model)
        {
            if (ModelState.IsValid==true)
            {
                var product = ProductRepository.getById(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Title = model.Title;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.catId;
                product.Quentity = model.Quentity;
                product.SellerId = model.sellerId;

                if (model.Image != null && model.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot", "Imges");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/Imges/" + uniqueFileName;
                }

                ProductRepository.update(product);
                ProductRepository.save();
                return RedirectToAction("Index");
            }

            ViewData["catList"] = CategoryRepository.getAll();
            model.ImageUrl = ProductRepository.getById(id)?.ImageUrl;
            return View(model);
        }



        public IActionResult Details(int id)
        {
            Product pro = ProductRepository.getById(id);
            if(pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Delete(int id)
        {
            Product pro = ProductRepository.getById(id);
            if (pro == null) { 
                return NotFound();
            }
            ProductRepository.delete(pro);
            ProductRepository.save();
            return RedirectToAction("Index");

        }
        //------------------------------------------------------------------------------


        [Authorize(Roles="Seller")]
        public IActionResult Orders() 
        {
            var sellerid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Seller ID: {sellerid}");
            var orders = orderService.GetAllOrdersBySellerId(sellerid);

            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }
        //STATIC ID
        //var orders = orderService.GetAllOrdersBySellerId("6D793D4A-A25C-46B8-A48F-A5183E7A0683");

        public IActionResult OrderDetail(int id)
        {
            var order = orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order); // هنبعت تفاصيل الطلب للـ View
        }
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus newStatus) 
        {
        
            orderService.UpdateOrderStatus(orderId, newStatus);
            orderService.Save();
         return   RedirectToAction("orders");

        
        }
        //brand_name
        [HttpGet]
        public async Task<IActionResult> profile() 
        {
         
            var userr= await user.GetUserAsync(User);
            if(userr == null){ return NotFound(); }
            var model = new brandvm()
            {
          

            };
            return View(model);

        }
        //[HttpPost]
        //public async Task<IActionResult> EditBrandName(brandvm model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var userr = await user.GetUserAsync(User);
        //    if (userr == null) return NotFound();

        //    userr.brand_name = model.brand_name;
        //    var result = await user.UpdateAsync(userr);

        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Profile"); // رجعيه لصفحة البروفايل بعد التعديل
        //    }

        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error.Description);
        //    }

        //    return View("profile", model);
        //}

    }
}
