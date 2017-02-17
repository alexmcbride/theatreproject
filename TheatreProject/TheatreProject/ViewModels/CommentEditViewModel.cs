using System.ComponentModel.DataAnnotations;

namespace TheatreProject.ViewModels
{
    public class CommentEditViewModel
    {
        [Required]
        [Display(Name = "Comment")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public string PostTitle { get; set; }
        public int PostId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string UserName { get; set; }
    }
}