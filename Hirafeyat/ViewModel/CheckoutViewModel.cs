using Hirafeyat.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hirafeyat.ViewModel
{
    public class CheckoutViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        
    }
}
