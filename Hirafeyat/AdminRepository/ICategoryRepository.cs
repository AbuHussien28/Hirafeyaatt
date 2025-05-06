using Hirafeyat.Models;

namespace Hirafeyat.AdminRepository
{
    public interface ICategoryRepository 
    {
        IEnumerable<Category> GetAll();
        Category? GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
