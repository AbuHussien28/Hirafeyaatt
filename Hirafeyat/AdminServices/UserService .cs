﻿using Hirafeyat.AdminRepository;
using Hirafeyat.Models;
using Hirafeyat.ViewModel.Admin;
using Microsoft.AspNetCore.Identity;
using X.PagedList;

namespace Hirafeyat.AdminServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repo;

        public UserService(IUserRepository repo)
        {
            this.repo = repo;
        }

        public async Task BatchToggleUserStatusAsync(List<string> userNames, bool activate)
        {
            await repo.ActivateUsersAsync(userNames, activate);
            
        }

        public async Task<IPagedList<UserCustomerAdminViewModel>> GetCustomersAsync(int page, int pageSize, string query = "")
        {
            var users = await repo.GetCustomersAsync(page, pageSize, query);


            var CustomerVM = users.Select(u => new UserCustomerAdminViewModel
            {
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                ProfileImage = u.ProfileImage,
                Address = u.Address,
                IsActive = !(u.LockoutEnabled && u.LockoutEnd > DateTimeOffset.Now),
                AccountCreatedDate = u.AccountCreatedDate
            }).ToList();
            return new StaticPagedList<UserCustomerAdminViewModel>(
                CustomerVM,
                users.PageNumber,
                users.PageSize,
                users.TotalItemCount);
        }

        public async Task<IPagedList<UserSellerAdminViewModel>> GetSellersAsync(int page, int pageSize, string query = "")
        {
            var users = await repo.GetSellersAsync(page, pageSize, query);


            var viewModels = users.Select(u => new UserSellerAdminViewModel
            {
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                ProfileImage = u.ProfileImage,
                Address = u.Address,
                IsActive = !(u.LockoutEnabled && u.LockoutEnd > DateTimeOffset.Now),
                AccountCreatedDate = u.AccountCreatedDate
            }).ToList();
            return new StaticPagedList<UserSellerAdminViewModel>(
                viewModels,
                users.PageNumber,
                users.PageSize,
                users.TotalItemCount);
        }

        public async Task<bool> ToggleUserStatus(string userName)
        {
            return await repo.ToggleUserStatus(userName);
        }
        public async Task<UserSellerAdminViewModel> SellerDetailsAsync(string username)
        {
            var user = await repo.SellerDetailsAsync(username);
            var result = new UserSellerAdminViewModel()
            {
                Address = user.Address,
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                AccountCreatedDate = user.AccountCreatedDate,
                IsActive = !(user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now)
            };
            return result;
        }
        public async Task<UserSellerAdminViewModel> CustomerDetailsAsync(string username)
        {
            var user = await repo.CustomerDetailsAsync(username);
            var result = new UserSellerAdminViewModel()
            {
                Address = user.Address,
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                AccountCreatedDate = user.AccountCreatedDate,
                IsActive = !(user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now)
            };
            return result;
        }
       
    }
}
