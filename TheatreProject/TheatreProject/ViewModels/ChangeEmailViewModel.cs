using System.ComponentModel.DataAnnotations;

namespace TheatreProject.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm Email")]
        [Compare("Email", ErrorMessage = "The email address and confirmation email do not match.")]
        public string EmailConfirm { get; set; }
    }
}