using Hirafeyat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.SellerServices
{
    public class ProductService : IProductRepository
    {
        HirafeyatContext context;
        public ProductService(HirafeyatContext context)
        {
            this.context = context;
        }
        public List<Product> getAll()
        {
            return context.Products.Include(p=>p.Category).ToList();
        }

        public Product getById(int id)
        {
            return context.Products.Where(p => p.Id == id).FirstOrDefault();
        }

        public Product getByCatId(int id)
        {
            return context.Products.Where(p => p.CategoryId == id).FirstOrDefault();
        }

        public void add(Product product)
        {
            context.Products.Add(product);
        }
        public void update(Product product)
        {
            context.Products.Update(product);
        }
        public void delete(Product product)
        {
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

        public List<Product> GetAllWithSeller()
        {
            return context.Products.Include(p => p.Seller).ToList();
        }

        public IEnumerable<Product> GetFilteredProducts(string searchText, List<string> selectedPrices)
        {
            // أولًا تحويل الـ IQueryable إلى List
            var products = context.Products
                .Include(p => p.Category)  // تضمين الـ Category
                .Include(p => p.ImageUrl)  // إذا كان لديك صور مرتبطة
                .ToList();  // تحويل إلى List بدلًا من IQueryable

            // تطبيق البحث بالاسم
            if (!string.IsNullOrEmpty(searchText))
            {
                products = products.Where(p => p.Title.Contains(searchText)).ToList();
            }

            // تطبيق الفلترة بناءً على النطاقات المختارة
            if (selectedPrices != null && selectedPrices.Count > 0)
            {
                if (!selectedPrices.Contains("all"))
                {
                    products = products.Where(p => selectedPrices.Any(price =>
                    {
                        var range = price.Split('-');
                        return range.Length == 2
                            && decimal.TryParse(range[0], out decimal min)
                            && decimal.TryParse(range[1], out decimal max)
                            && p.Price >= min && p.Price <= max;
                    })).ToList();
                }
            }

            return products;
        }

       

    }
}
