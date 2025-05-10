using Hirafeyat.Models;
using Hirafeyat.ViewModel.Admin;
using X.PagedList;

namespace Hirafeyat.AdminServices
{
    public interface IUserService
    {
        Task<IPagedList<UserSellerAdminViewModel>> GetSellersAsync(int page, int pageSize, string query = "");
        Task<IPagedList<UserCustomerAdminViewModel>> GetCustomersAsync(int page, int pageSize, string query = "");
        Task<bool> ToggleUserStatus(string userName);
        Task BatchToggleUserStatusAsync(List<string> userNames, bool activate);
        Task<UserSellerAdminViewModel> SellerDetailsAsync(string username);
        Task<UserSellerAdminViewModel> CustomerDetailsAsync(string username);
    }
}
