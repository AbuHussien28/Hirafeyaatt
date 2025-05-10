using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Hirafeyat.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            var cat = await GetByIdAsync(id);
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

        public async Task<Category?> GetByIdAsync(int id)
        {
            var cat = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            return cat != null ? cat : null;
        }
        public IEnumerable<Category> GetByNameAsync(string query)
        {
            var categories = string.IsNullOrWhiteSpace(query)
            ? _context.Categories.ToList()
                : _context.Categories.Where(c => c.Name.Contains(query)).ToList();
            return categories;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var cat = await GetByIdAsync(category.Id);
            if (cat != null)
            {
                cat.Name = category.Name;
                _context.Categories.Update(cat);
                await SaveAsync();
            }
        }
    }
}
