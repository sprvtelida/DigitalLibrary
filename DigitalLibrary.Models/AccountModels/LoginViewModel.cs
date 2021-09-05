
using System.ComponentModel.DataAnnotations;

namespace DigitalLibrary.Models.AccountModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Required")]
        public string EmailOrLogin { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }
        [Required]
        public string ReturnUrl { get; set; }
    }
}
