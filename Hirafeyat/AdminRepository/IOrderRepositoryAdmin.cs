using Hirafeyat.Models;


namespace Hirafeyat.AdminRepository
{
    public interface IOrderRepositoryAdmin
    {
        Task<IPagedList<Order>> GetAllOrdersByCustomerAsync(int page, int pageSize, string statusFilter = null, string categoryFilter = null,
            DateTime? startDateFilter = null,
            DateTime? endDateFilter = null,
            string productFilter = null);
    }
}
