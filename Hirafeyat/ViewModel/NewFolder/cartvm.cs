namespace Hirafeyat.ViewModel.NewFolder
{
    public class cartvm
    {
        public List<CartItemViewModel> Items { get; set; }
        public decimal CartTotal => Items?.Sum(x => x.Total) ?? 0;
    }
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }



}
