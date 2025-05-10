using Hirafeyat.Models;

namespace Hirafeyat.AdminRepository
{
    public interface ICategoryRepository 
    {
        IEnumerable<Category> GetAll();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task SaveAsync();
        IEnumerable<Category> GetByNameAsync(string query);
       
    }
}
