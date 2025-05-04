using Microsoft.AspNetCore.Identity;


namespace Hirafeyat.ViewModel.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public IFormFile ImageFile { get; set; } 
        public string? imagePath { get; set; } 
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage ="Address Is Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }
        public string? BrandName { get; set; }
    }
}
