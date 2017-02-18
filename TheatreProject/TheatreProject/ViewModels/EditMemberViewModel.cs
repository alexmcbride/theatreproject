using System.ComponentModel.DataAnnotations;

namespace TheatreProject.ViewModels
{
    public class EditMemberViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        public bool EmailConfirmed { get; set; }
    }
}