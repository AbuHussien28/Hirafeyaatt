﻿using Hirafeyat.AdminRepository;
using Hirafeyat.ViewModel.Admin;
using Hirafeyat.ViewModel.Admin.OrderManagement;
using X.PagedList;

namespace Hirafeyat.AdminServices
{
    public class OrderAdminService : IOrderAdminService
    {
        private readonly IOrderRepositoryAdmin orderRepoAdmin;

        public OrderAdminService(IOrderRepositoryAdmin orderRepoAdmin)
        {
            this.orderRepoAdmin = orderRepoAdmin;
        }

        public async Task<IPagedList<AdminOrderViewModel>> GetAllOrdersByCustomerAsync(int page, int pageSize, string statusFilter = null, string categoryFilter = null,
    DateTime? startDateFilter = null,
    DateTime? endDateFilter = null,
    string productFilter = null)
        {
            var orders = await orderRepoAdmin.GetAllOrdersByCustomerAsync(page, pageSize, statusFilter, categoryFilter, startDateFilter, endDateFilter, productFilter);


            var orderVM = orders.Select(o => {
                var firstItem = o.OrderItems.FirstOrDefault();

                return new AdminOrderViewModel
                {
                    SellerFullName = firstItem?.Product?.Seller?.FullName,
                    CustomerFullName = o.Customer.FullName,
                    CustomerEmail = o.Customer.Email,
                    ProductTitle = firstItem?.Product?.Title,
                    Quantity = firstItem?.Quantity ?? 0,
                    ProductPrice = firstItem?.Product?.Price ?? 0,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    DeliveryAddress = o.Address
                };
            }).ToList();
            return new StaticPagedList<AdminOrderViewModel>(
                orderVM,
                orders.PageNumber,
                orders.PageSize,
                orders.TotalItemCount);
        }
    }
}
