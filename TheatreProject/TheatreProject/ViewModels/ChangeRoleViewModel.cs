using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TheatreProject.ViewModels
{
    public class ChangeRoleViewModel
    {
        public ICollection<SelectListItem> Roles { get; set; }

        [Display(Name = "Role")]
        public string NewRole { get; set; }
    }
}