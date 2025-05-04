using Hirafeyat.Models;
using Hirafeyat.ViewModel.Admin;
using X.PagedList;

namespace Hirafeyat.AdminServices
{
    public interface IUserService
    {
        Task<IPagedList<UserSellerAdminViewModel>> GetSellersAsync(int page, int pageSize);
        Task<IPagedList<UserCustomerAdminViewModel>> GetCustomersAsync(int page, int pageSize);
        Task<bool> ToggleUserStatus(string userName);
        Task BatchToggleUserStatusAsync(List<string> userNames, bool activate);
    }
}
