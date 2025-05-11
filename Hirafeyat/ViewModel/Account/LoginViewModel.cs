namespace Hirafeyat.ViewModel.Account
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        // This property is needed for return URLs
        public string? ReturnUrl { get; set; } = null;
    }
}
