using System.ComponentModel.DataAnnotations;

namespace Hirafeyat.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
