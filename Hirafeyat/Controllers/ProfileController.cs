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
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ProfileController(UserManager<ApplicationUser> userManager, IProductRepository productRepo, SignInManager<ApplicationUser> _signInManager)
        {
            _userManager = userManager;
            productRepository = productRepo;
            this._signInManager = _signInManager;
        }

        // 3/5
        [HttpGet]
        [Authorize]
        public IActionResult ChangeProfilePhoto(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                ViewBag.CurrentPhoto = imagePath;
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeProfilePhoto(IFormFile ProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();


            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imges");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfileImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImage = "/Imges/" + uniqueFileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index", "Home");

        }


        //----------------------------------------------------------

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePersonalInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ChangeProfileViewModel
            {
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePersonalInfo(ChangeProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }

        //----------------------------------------------------------

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password changed successfully";
                return View("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }


        // 3/5


    }
}
