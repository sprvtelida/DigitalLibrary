using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.AccountModels
{
    public class PasswordRestoreModel
    {
        public string ReturnUrl { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordsNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
