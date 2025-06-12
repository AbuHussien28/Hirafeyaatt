namespace Hirafeyat.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }


        public int Quantity { get; set; } = 1;

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
