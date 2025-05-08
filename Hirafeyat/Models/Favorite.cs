namespace Hirafeyat.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
