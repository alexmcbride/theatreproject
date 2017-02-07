using System.ComponentModel.DataAnnotations;

namespace TheatreProject.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}