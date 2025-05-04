namespace Hirafeyat.SellerServices
{
    public interface IRepository<T>
    {
        public List<T> getAll();
        public T getById(int id);
        public void add(T item);
        public void update(T item);
        public void delete(T item);
        public int save();
    }
}
