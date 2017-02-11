using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TheatreProject.ViewModels
{
    public class ChangeRoleViewModel
    {
        public ICollection<SelectListItem> Roles { get; set; }

        public string NewRole { get; set; }
    }
}