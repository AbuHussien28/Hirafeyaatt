namespace Hirafeyat.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
