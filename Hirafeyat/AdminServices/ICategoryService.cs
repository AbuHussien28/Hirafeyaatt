using Hirafeyat.Models;

namespace Hirafeyat.AdminServices
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAll();
        Category? GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task Create(Category category);
    }
}
