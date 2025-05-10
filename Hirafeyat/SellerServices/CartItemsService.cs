using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public class CartItemsService:ICartItemsRepository
    {
        HirafeyatContext context;
        public CartItemsService(HirafeyatContext context)
        {
            this.context = context;
        }
        public List<CartItem> getAll()
        {
            return context.CartItems.ToList();
        }

        public CartItem getById(int id)
        {
            return context.CartItems.Where(p => p.Id == id).FirstOrDefault();
        }



        public void add(CartItem product)
        {
            context.CartItems.Add(product);
        }
        public void update(CartItem product)
        {
            context.CartItems.Update(product);
        }
        public void delete(CartItem product)
        {
            context.CartItems.Remove(product);
        }

        public int save()
        {
            return context.SaveChanges();
        }

        public IQueryable<CartItem> getAllWithoutLoading()
        {
            return context.CartItems;
        }
    }
}
