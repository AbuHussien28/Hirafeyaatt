namespace Hirafeyat.ViewModel
{
    public class NewProductViewModel
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quentity { get; set; }
        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int catId { get; set; }
        public string sellerId { get; set; }

    }
}
