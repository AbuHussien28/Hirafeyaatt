using Hirafeyat.Models;
using X.PagedList;

namespace Hirafeyat.AdminRepository
{
    public interface IUserRepository
    {
        Task<IPagedList<ApplicationUser>> GetSellersAsync(int page, int pageSize, string query = "");
        Task<IPagedList<ApplicationUser>>GetCustomersAsync(int page, int pageSize, string? searchTerm = null);
        Task<bool> ToggleUserStatus(string userName);
        Task ActivateUsersAsync(List<string> userNames, bool activate);
        Task<ApplicationUser> SellerDetailsAsync(string username);
        Task<ApplicationUser> CustomerDetailsAsync(string username);
    }
}
