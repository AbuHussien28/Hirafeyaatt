using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public class ProductService:IProductRepository
    {
        HirafeyatContext context;
        public ProductService(HirafeyatContext context) {
            this.context = context;
        }
        public List<Product> getAll()
        {
            return context.Products.ToList();
        }

        public Product getById(int id) {
            return context.Products.Where(p => p.Id == id).FirstOrDefault();
        }

        public Product getByCatId(int id) {
            return context.Products.Where(p => p.CategoryId == id).FirstOrDefault();
        }

        public void add(Product product) { 
            context.Products.Add(product);
        }
        public void update(Product product) { 
            context.Products.Update(product);
        }
        public void delete(Product product) { 
            context.Products.Remove(product);
        }

        public int save()
        {
            return context.SaveChanges();
        }
        public List<Product> getProductsBySellerId(string sellerId)
        {
            return context.Products.Where(p => p.SellerId == sellerId).ToList();
        }
    }
}
