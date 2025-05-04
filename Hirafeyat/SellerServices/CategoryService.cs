using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public class CategoryService:ICategoryRepository
    {
        HirafeyatContext context;
        public CategoryService(HirafeyatContext context) { 
            this.context = context;
        }
        public List<Category> getAll() { 
            return context.Categories.ToList();
        }
        public Category getById(int id) {
            return context.Categories.Where(c => c.Id == id).FirstOrDefault();
        }

        public void add(Category category) { 
            context.Categories.Add(category);
        }
        public void update(Category category)
        {
            context.Categories.Update(category);
        }

        public void delete(Category category) { 
            context.Categories.Remove(category);
        }
        public int save()
        {
            return context.SaveChanges();
        }
    }
}
