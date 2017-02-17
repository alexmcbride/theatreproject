using System;
using System.ComponentModel.DataAnnotations;

namespace TheatreProject.ViewModels
{
    public class CommentDeleteViewModel
    {
        [Display(Name = "Post")]
        public string PostTitle { get; set; }
        public int PostId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        public DateTime Posted { get; set; }
        public string Email { get; set; }
    }
}