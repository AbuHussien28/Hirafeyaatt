using Hirafeyat.Models;
using Hirafeyat.ViewModel.Admin;
using Microsoft.EntityFrameworkCore;

namespace Hirafeyat.AdminRepository
{
    public interface IProductRepository 
    {
        Task<IPagedList<Product>> GetProductsAsync(int pageNumber, int pageSize, string? sellerId = null);
        Task<IPagedList<Product>> GetProductsByNameAsync(int pageNumber, int pageSize, string? name = null);
        Task<int> GetTotalProductsCountAsync(string? sellerId = null);
        Task<Product> GetProductByIdAsync(int id);
        Task UpdateProductAsync(Product product);
        public Task DeleteProductAsync(int id);
        Task SaveAsync();
    }
}
