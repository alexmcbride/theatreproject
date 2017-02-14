using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheatreProject.Models;

namespace TheatreProject.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        public IList<Comment> Comments { get; set; }
    }
}