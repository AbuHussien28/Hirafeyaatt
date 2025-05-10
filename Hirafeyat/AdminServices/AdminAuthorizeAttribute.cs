using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hirafeyat.AdminServices
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public AdminAuthorizeAttribute()
        {
            Roles = "Admin";
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Redirect to login page with return URL
                string returnUrl = context.HttpContext.Request.Path;
                context.Result = new RedirectToActionResult("Login", "Account", new { ReturnUrl = returnUrl });
                return;
            }

            // Check if user is in Admin role
            if (!context.HttpContext.User.IsInRole("Admin"))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }
        }
    }

    // Custom authorization attribute for Seller
    public class SellerAuthorizeAttribute : AuthorizeAttribute
    {
        public SellerAuthorizeAttribute()
        {
            Roles = "Seller";
        }
    }

    // Custom authorization attribute for Customer
    public class CustomerAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomerAuthorizeAttribute()
        {
            Roles = "Customer";
        }
    }
}
