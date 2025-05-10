using Hirafeyat.Models;

namespace Hirafeyat.SellerServices
{
    public class OrderItemsService:IOrderItemsRepository
    {
        HirafeyatContext context;
        public OrderItemsService(HirafeyatContext context)
        {
            this.context = context;
        }
        public List<OrderItem> getAll()
        {
            return context.OrderItems.ToList();
        }

        public OrderItem getById(int id)
        {
            return context.OrderItems.Where(p => p.Id == id).FirstOrDefault();
        }

        

        public void add(OrderItem product)
        {
            context.OrderItems.Add(product);
        }
        public void update(OrderItem product)
        {
            context.OrderItems.Update(product);
        }
        public void delete(OrderItem product)
        {
            context.OrderItems.Remove(product);
        }

        public int save()
        {
            return context.SaveChanges();
        }

        public IQueryable<OrderItem> getAllWithoutLoading()
        {
            return context.OrderItems;
        }


    }
}
