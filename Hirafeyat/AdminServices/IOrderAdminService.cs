using Hirafeyat.ViewModel.Admin;
using Hirafeyat.ViewModel.Admin.OrderManagement;
using X.PagedList;

namespace Hirafeyat.AdminServices
{
    public interface IOrderAdminService
    {
        Task<IPagedList<AdminOrderViewModel>> GetAllOrdersByCustomerAsync(int page,
    int pageSize ,
    string statusFilter = null,
    string categoryFilter = null,
    DateTime? startDateFilter = null,
    DateTime? endDateFilter = null,
    string productFilter = null);
    }
}
