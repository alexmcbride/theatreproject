using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TheatreProject.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserName { get; set; }

        public ICollection<SelectListItem> Roles { get; set; }

        [Required, Display(Name = "Role")]
        public string Role { get; set; }
    }
}