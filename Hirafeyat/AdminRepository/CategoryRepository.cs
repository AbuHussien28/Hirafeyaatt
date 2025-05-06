using System.Data.Entity;
using Hirafeyat.Models;

namespace Hirafeyat.AdminRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly HirafeyatContext _context;
        public CategoryRepository(HirafeyatContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                await SaveAsync();
            }
        }

        public IEnumerable<Category> GetAll()
        {
             return _context.Categories.ToList();
        }

        public Category? GetByIdAsync(int id)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == id);
            return cat != null ? cat : null;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (cat != null)
            {
                _context.Categories.Update(cat);
                await SaveAsync();
            }
        }
    }
}
