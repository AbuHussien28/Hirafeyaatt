namespace Hirafeyat.ViewModel.Admin
{
    public class UserSellerAdminViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } 
        public DateTime? AccountCreatedDate { get; set; }
    }
}
