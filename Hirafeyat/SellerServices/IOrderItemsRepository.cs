using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public interface IOrderItemsRepository:IRepository<OrderItem>
    {
        public IQueryable<OrderItem> getAllWithoutLoading();
    }
}
