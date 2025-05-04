namespace Hirafeyat.ViewModel.Account.ForgetPassword
{
    public class ForgotPasswordViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
    }
}
