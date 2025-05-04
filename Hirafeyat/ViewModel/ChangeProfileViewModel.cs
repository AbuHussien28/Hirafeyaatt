namespace Hirafeyat.ViewModel
{
    public class ChangeProfileViewModel
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        [Display(Name = "Current Profile Image")]
        public string? ProfileImageUrl { get; set; } 

        [Display(Name = "New Profile Image")]
        public IFormFile? ProfileImage { get; set; }
    }
}
