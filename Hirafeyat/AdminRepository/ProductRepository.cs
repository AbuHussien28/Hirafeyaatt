using Hirafeyat.Controllers;
using Hirafeyat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Hirafeyat.AdminRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly HirafeyatContext _context;
        public ProductRepository(HirafeyatContext context) 
        {
            _context = context;
        }
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Product> GetProductByIdAsync(int id) 
        {
            return await _context.Products.Include(p => p.Category).Include(p => p.Seller).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IPagedList<Product>> GetProductsAsync(int page, int pageSize, string? sellerId = null)
        {
            var query = _context.Products.Include(p => p.Seller).AsQueryable();

            if (!string.IsNullOrEmpty(sellerId))
            {
                query = query.Where(p => p.Seller.Id == sellerId);
            }

            var totalCount = await query.CountAsync();
            var productsData = await query
                .OrderByDescending(x => x.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<Product>(productsData, page, pageSize, totalCount);
        }

        public async Task<int> GetTotalProductsCountAsync(string? sellerId = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(sellerId))
            {
                query = query.Where(p => p.Seller.Id == sellerId);
            }

            return await query.CountAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
