
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hirafeyat.Controllers.Role
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult NewRole()
        {
            return View("NewRole");
        }
        [HttpPost]
        public async Task<IActionResult> NewRole(RoleViewModel roleVM)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole() 
                {
                    Name = roleVM.RoleName
                };
                IdentityResult resultRole=await roleManager.CreateAsync(role);
                if (resultRole != null) 
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach (var item in resultRole.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View("NewRole", roleVM);
        }
    }
}
