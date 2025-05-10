using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public interface ICartItemsRepository:IRepository<CartItem>
    {
        public IQueryable<CartItem> getAllWithoutLoading();

    }
}
