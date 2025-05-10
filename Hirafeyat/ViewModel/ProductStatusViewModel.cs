using Hirafeyat.Models;

namespace Hirafeyat.ViewModel
{
    public class ProductStatusViewModel
    {
        public int Id { get; set; }

        [Required]
        public productStatus? Status { get; set; }
        
        public string? Title { get; set; }
        
        public string? ImageUrl { get; set; }

    }
}
