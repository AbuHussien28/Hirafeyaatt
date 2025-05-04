using Hirafeyat.Models;
using X.PagedList;

namespace Hirafeyat.AdminRepository
{
    public interface IUserRepository
    {
        Task<IPagedList<ApplicationUser>> GetSellersAsync(int page, int pageSize);
        Task<IPagedList<ApplicationUser>>GetCustomersAsync(int page, int pageSize);
        Task<bool> ToggleUserStatus(string userName);
        Task ActivateUsersAsync(List<string> userNames, bool activate);
    }
}
