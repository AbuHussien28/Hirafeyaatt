using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public interface IProductRepository:IRepository<Product>
    {
        Product getByCatId(int catId);
        List<Product> getProductsBySellerId(string sellerId);
    }
}
