using System.Net;
using Hirafeyat.Models;
using Hirafeyat.SellerServices;
using Hirafeyat.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Hirafeyat.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductRepository productRepository;
        public ProfileController(UserManager<ApplicationUser> userManager , IProductRepository productRepo)
        {
            _userManager = userManager;
            productRepository = productRepo;
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProfileInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                ChangeProfileViewModel userFromDb = new ChangeProfileViewModel()
                {
                    FullName = user.FullName,
                    Address = user.Address,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ProfileImageUrl = user.ProfileImage
                };
                
                return View(userFromDb);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller,Customer,Admin")]
        public async Task<IActionResult> ChangeProfileInfo(ChangeProfileViewModel userFromReq)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                user.FullName = userFromReq.FullName;
                user.Address = userFromReq.Address;
                user.Email = userFromReq.Email;
                user.PhoneNumber = userFromReq.PhoneNumber;

                if (userFromReq.ProfileImage != null && userFromReq.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imges");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + userFromReq.ProfileImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await userFromReq.ProfileImage.CopyToAsync(fileStream);
                    }

                    user.ProfileImage = "/Imges/" + uniqueFileName;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    productRepository.save();
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            var currentUser = await _userManager.GetUserAsync(User);
            userFromReq.ProfileImageUrl = currentUser?.ProfileImage;

            return View(userFromReq);
        }
    }
}
