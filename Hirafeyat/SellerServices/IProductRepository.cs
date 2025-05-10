using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public interface IProductRepository : IRepository<Product>
    {
        Product getByCatId(int catId);
        List<Product> getProductsBySellerId(string sellerId);

        List<Product> GetAllWithSeller();

        IEnumerable<Product> GetFilteredProducts(string searchText, List<string> selectedPrices);


    }
}
