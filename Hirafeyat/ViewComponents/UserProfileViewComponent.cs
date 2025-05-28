using Hirafeyat.ViewModel.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hirafeyat.ViewComponents
{
    public class UserProfileViewComponent :ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserProfileViewComponent(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ApplicationUser currentUser = null;
            string userProfileImageUrl = "/images/default-profile.png";
            string userName = "User";

            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                currentUser = await _userManager.GetUserAsync(HttpContext.User);

                if (currentUser != null)
                {
                    userName = currentUser.UserName ?? "User";
                    userProfileImageUrl = currentUser.ProfileImage ?? userProfileImageUrl;
                }
            }

            var viewModel = new UserProfileViewModel
            {
                UserName = userName,
                ProfileImage = userProfileImageUrl,
            };

            return View(viewModel);
        }
    }
}
