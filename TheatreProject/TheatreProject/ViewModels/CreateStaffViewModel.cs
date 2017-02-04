using System.ComponentModel.DataAnnotations;

namespace TheatreProject.ViewModels
{
    public class CreateStaffViewModel : EditStaffViewModel
    {
        [Required, DataType(DataType.Password), Compare("PasswordConfirm")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }
    }
}