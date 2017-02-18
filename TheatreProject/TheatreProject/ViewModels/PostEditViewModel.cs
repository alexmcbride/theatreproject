using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TheatreProject.Models;

namespace TheatreProject.ViewModels
{
    public class PostEditViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public SelectList Categories { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } 
    }
}