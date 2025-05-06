using System.Threading.Tasks;
using Hirafeyat.AdminRepository;
using Hirafeyat.Models;
using NuGet.Protocol.Core.Types;

namespace Hirafeyat.AdminServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task AddAsync(Category category)
        {
          await _categoryRepository.AddAsync(category);
        }
        public async Task Create(Category category)
        {
            await _categoryRepository.AddAsync(category);
        }
        public async Task DeleteAsync(int id)
        {
           await _categoryRepository.DeleteAsync(id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _categoryRepository.GetAll();
        }

        public Category? GetByIdAsync(int id)
        {
            return _categoryRepository?.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
        }
    }
}
