using Microsoft.AspNetCore.Identity;

namespace Hirafeyat.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string ProfileImage { get; set; }
        public DateTime? AccountCreatedDate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

      

    }
}
