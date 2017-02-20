using System;
using System.ComponentModel.DataAnnotations;

namespace TheatreProject.ViewModels
{
    public class EditStaffViewModel : EditProfileViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Confirm Email")]
        public bool EmailConfirmed { get; set; }
    }
}