using System.Security.Claims;
using Hirafeyat.OtherServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.Controllers.LoginAndRegister
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }
        #region Register

        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This Email is Already In DataBase");
                return View("Register", model);
            }

            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    FullName = model.FullName,
                    AccountCreatedDate = DateTime.Now,
                };

                if (model.ImageFile != null)
                {
                    userFromDb.ProfileImage = ImageUploader.UploadImage(model.ImageFile, "Imges");
                }
                IdentityResult result = await userManager.CreateAsync(userFromDb, model.Password);

                if (result.Succeeded)
                {
                    // Add external login if exists
                    if (TempData["LoginProvider"] != null && TempData["ProviderKey"] != null)
                    {
                        var info = new UserLoginInfo(
                            TempData["LoginProvider"].ToString(),
                            TempData["ProviderKey"].ToString(),
                            TempData["LoginProvider"].ToString());

                        await userManager.AddLoginAsync(userFromDb, info);
                    }
                    var roleResult = await userManager.AddToRoleAsync(userFromDb, model.Role);
                    if (!roleResult.Succeeded)
                    {
                        await userManager.DeleteAsync(userFromDb);
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("Register", model);
                    }
                    if (model.Role == "Seller" && !string.IsNullOrEmpty(model.BrandName))
                    {
                        await userManager.AddClaimAsync(userFromDb,
                            new Claim("BrandName", model.BrandName));
                    }
                    await signInManager.SignInAsync(userFromDb, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("Register", model);
        }
        #endregion
        #region Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        #endregion
        #region Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var roles = await userManager.GetRolesAsync(user);
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("FullName", user.FullName ?? string.Empty),
                    new Claim("ProfileImage", user.ProfileImage ?? "/images/default-profile.png")
                };
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        if (roles.Contains("Seller"))
                        {
                            var brandClaim = (await userManager.GetClaimsAsync(user))
                                .FirstOrDefault(c => c.Type == "BrandName");
                            if (brandClaim != null)
                            {
                                claims.Add(new Claim("BrandName", brandClaim.Value));
                            }
                        }
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
                            });
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else if (roles.Contains("Seller"))
                        {
                            return RedirectToAction("Index", "Seller");
                        }
                        else if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            return View(model);
        }
        #endregion
        #region Google SignIn
        [HttpGet]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalLoginCallback(string? remoteError = null)
        {


            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email != null)
            {
                TempData["ExternalEmail"] = email;
                TempData["LoginProvider"] = info.LoginProvider;
                TempData["ProviderKey"] = info.ProviderKey;

                return RedirectToAction("RegisterFromExternal");
            }

            return RedirectToAction(nameof(Login));
        }
        public IActionResult RegisterFromExternal()
        {
            var email = TempData["ExternalEmail"]?.ToString();
            var model = new RegisterViewModel
            {
                Email = email
            };

            return View("Register", model);
        }

        #endregion
        #region UpdateBrandName
        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> UpdateBrandName()
        {
            var user = await userManager.GetUserAsync(User);
            var claims = await userManager.GetClaimsAsync(user);
            var brandClaim = claims.FirstOrDefault(c => c.Type == "BrandName");

            var model = new UpdateBrandViewModel
            {
                BrandName = brandClaim?.Value
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateBrandName(UpdateBrandViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var claims = await userManager.GetClaimsAsync(user);

                var oldClaim = claims.FirstOrDefault(c => c.Type == "BrandName");

                if (oldClaim != null)
                {
                    await userManager.RemoveClaimAsync(user, oldClaim);
                }
                await userManager.AddClaimAsync(user, new Claim("BrandName", model.BrandName));

                TempData["Success"] = "Brand Name updated successfully!";
                return RedirectToAction("UpdateBrandName");
            }
            return View(model);
        }
        #endregion
        #region ForGet Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("ForgetPassword/ForgotPassword");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);
                string subject = "Reset Your Password";
                string body = $"<p>To reset your password, please click the following link:</p><p><a href='{callbackUrl}'>Reset Password</a></p>";

                await emailSender.SendEmailAsync(model.Email, subject, body);



                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View("ForgetPassword/ForgotPasswordConfirmation");
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Login");
            }
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View("ForgetPassword/ResetPassword", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View("ForgetPassword/ResetPasswordConfirmation");
        }

        #endregion
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
