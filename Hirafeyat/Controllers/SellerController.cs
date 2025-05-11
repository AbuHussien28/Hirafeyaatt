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
using Hirafeyat.OtherServices;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.Controllers
{
    [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly IOrderService orderService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> user;
        private readonly IProductRepository productRepo;
        private readonly ICategoryRepository catRepo;
        private readonly IOrderItemsRepository orderItemsRepository;

        public SellerController(IOrderService orderService,UserManager<ApplicationUser> user , IProductRepository productRepo ,
            SellerServices.ICategoryRepository catRepo, HirafeyatContext context, SellerServices.IOrderItemsRepository orderItemsRepository)
        {
            this.orderService = orderService; 
            this.orderService = orderService;
            this.user = user;
            this.productRepo = productRepo;
            this.catRepo = catRepo;
            this.orderItemsRepository = orderItemsRepository;
       
            this.orderItemsRepository = orderItemsRepository;
        }
        public IActionResult Index()
        {
            var sId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            List<Product> products = productRepo.getProductsBySellerId(sId);
            return View("Index" , products);
        }

        public IActionResult New()
        {
            ViewData["CatList"] = catRepo.getAll() ;
            return View("New");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewProductViewModel productfromModel)
        {
            if (ModelState.IsValid == true) {
                string imageUrl = null;

                var product = new Product
                    {
                        Title = productfromModel.Title,
                        Description = productfromModel.Description,
                        Price = (int)productfromModel.Price,
                    Quentity = productfromModel.Quentity,
                        ImageUrl = imageUrl,
                        CategoryId = productfromModel.catId,
                        SellerId = productfromModel.sellerId,
                        CreatedAt = DateTime.Now
                    };
                    try
                    {
                    if (productfromModel.Image != null)
                    {
                        product.ImageUrl = ImageUploader.UploadImage(productfromModel.Image, "Imges");
                    }
                    productRepo.add(product);
                        productRepo.save();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("other", ex.InnerException.Message);

                    }
                }
            
            
            ViewData["catList"] = catRepo.getAll();
            return View(productfromModel);
        }

        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = productRepo.getById(id);
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
            ViewData["catList"]= catRepo.getAll();
            return View(productToEdit);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewProductViewModel model)
        {
            if (ModelState.IsValid==true)
            {
                var product = productRepo.getById(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Title = model.Title;
                product.Description = model.Description;
                product.Price = (int)model.Price;
                product.CategoryId = model.catId;
                product.Quentity = model.Quentity;
                product.SellerId = model.sellerId;

                if (model.Image != null && model.Image.Length > 0)
                {
                        product.ImageUrl = ImageUploader.UploadImage(model.Image, "Imges");
                }

                productRepo.update(product);
                productRepo.save();
                return RedirectToAction("Index");
            }

            ViewData["catList"] = catRepo.getAll();
            model.ImageUrl = productRepo.getById(id)?.ImageUrl;
            return View(model);
        }



        public IActionResult Details(int id)
        {
            var product = productRepo.getById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Delete(int id)
        {
            Product pro = productRepo.getById(id);
            if (pro == null) { 
                return NotFound();
            }
            productRepo.delete(pro);
            productRepo.save();
            return RedirectToAction("Index");

        }
        

        public IActionResult Orders()
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = orderService.GetAllOrdersBySellerId(sellerId);

            var sellerOrders = orders.Select(order => new SellerOrderViewModel
            {
                OrderId = order.Id,
                CustomerName = order.FullName,
                OrderDate = order.OrderDate,
                Address = order.Address,
                Status = order.Status,
                Items = order.OrderItems
                    .Where(oi => oi.Product.SellerId == sellerId)
                    .Select(oi => new SellerOrderItemViewModel
                            {
                            OrderItemId = oi.Id,
                            ProductId = oi.Product.Id, // ✅ هنا
                            ProductTitle = oi.Product.Title,
                            Quantity = oi.Quantity,
                            ProductStatus = oi.ItemStatus
                            }).ToList()
                    }).ToList();


            return View(sellerOrders);
        }


        [Authorize(Roles = "Seller")]
        public IActionResult OrderDetail(int id)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = orderService.getAllWithoutLoading()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.SellerId = sellerId;
            return View(order);
        }



        [HttpPost]
        [Authorize(Roles = "Seller")]
        public IActionResult UpdateProductStatus(int orderItemId, OrderStatus newStatus)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderItem = orderItemsRepository.getAllWithoutLoading()
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .FirstOrDefault(oi => oi.Id == orderItemId);

            if (orderItem == null || orderItem.Product.SellerId != sellerId)
            {
                return Unauthorized();
            }

            orderItem.ItemStatus = newStatus;
            orderItemsRepository.update(orderItem);

            // حدث حالة الأوردر بعد تغيير حالة أحد العناصر
            orderService.UpdateOrderStatusByProducts(orderItem.OrderId);

            //context.SaveChanges();
            orderItemsRepository.save();

            return RedirectToAction("Orders");
        }



        [HttpPost]
        [Authorize(Roles = "Seller")]
        public IActionResult RemoveOrder(int orderId)
        {
            var order = orderService.getById(orderId);
            if (order == null)
                return NotFound();

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Delivered)
            {
                orderService.delete(order);
                orderService.save();
            }

            return RedirectToAction("Orders");
        }




    }
}
