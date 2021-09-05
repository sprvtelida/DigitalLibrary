using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.AccountModels
{
    public class RegistrationViewModel
    {
        public string ReturnUrl { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress (ErrorMessage = "EmailAddress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordsNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
