using Hirafeyat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using X.PagedList;

namespace Hirafeyat.AdminRepository
{
    public class UserRepository:IUserRepository
    {
        private readonly HirafeyatContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(HirafeyatContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        public async Task ActivateUsersAsync(List<string> userNames, bool activate)
        {
            var users = await _context.Users.Where(u => userNames.Contains(u.UserName)).ToListAsync();

            foreach (var user in users)
            {
                if (activate)
                {
                    user.LockoutEnd = DateTimeOffset.Now;
                    user.LockoutEnabled = false;
                }
                else
                {
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                    user.LockoutEnabled = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IPagedList<ApplicationUser>> GetCustomersAsync(int page, int pageSize, string query = "")
        {

            var sellerQuery = _context.Users
                .Join(_context.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
                    (u, ur) => new { User = u, ur.RoleId })
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.User, RoleName = r.Name })
                .Where(x => x.RoleName == "Customer");

            // لو فيه كلمة بحث نفلتر
            if (!string.IsNullOrEmpty(query))
            {
                sellerQuery = sellerQuery.Where(x =>
                    x.User.UserName.Contains(query) ||
                    x.User.Email.Contains(query));
            }

            var totalCount = await sellerQuery.CountAsync();

            var users = await sellerQuery
                .OrderBy(x => x.User.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.User)
                .ToListAsync();

            return new StaticPagedList<ApplicationUser>(users, page, pageSize, totalCount);
        }


        public async Task<IPagedList<ApplicationUser>> GetSellersAsync(int page, int pageSize, string query = "")
        {
            
            var sellerQuery = _context.Users
                .Join(_context.UserRoles,
                    u => u.Id,
                    ur => ur.UserId,
                    (u, ur) => new { User = u, ur.RoleId })
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.User, RoleName = r.Name })
                .Where(x => x.RoleName == "Seller");

            // لو فيه كلمة بحث نفلتر
            if (!string.IsNullOrEmpty(query))
            {
                sellerQuery = sellerQuery.Where(x =>
                    x.User.UserName.Contains(query) ||
                    x.User.Email.Contains(query));
            }

            var totalCount = await sellerQuery.CountAsync();

            var users = await sellerQuery
                .OrderBy(x => x.User.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.User)
                .ToListAsync();

            return new StaticPagedList<ApplicationUser>(users, page, pageSize, totalCount);
        }

        public async Task<bool> ToggleUserStatus(string userName)
        {
            var userFromDB=await userManager.FindByNameAsync(userName);
            if (userFromDB == null) return false;
            if (userFromDB.LockoutEnabled && userFromDB.LockoutEnd > DateTimeOffset.Now)
            {
                userFromDB.LockoutEnd = DateTimeOffset.Now;
                userFromDB.LockoutEnabled = false;
            }
            else
            {
                userFromDB.LockoutEnd = DateTimeOffset.MaxValue;
                userFromDB.LockoutEnabled = true;
            }
            var resultStatus= await userManager.UpdateAsync(userFromDB);
            return resultStatus.Succeeded;
        }
        public async Task<ApplicationUser> SellerDetailsAsync(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user != null && await userManager.IsInRoleAsync(user, "Seller"))
            {
                return user;
            }

            return null;
        }
        public async Task<ApplicationUser> CustomerDetailsAsync(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user != null && await userManager.IsInRoleAsync(user, "Customer"))
            {
                return user;
            }

            return null;
        }


    }
}
