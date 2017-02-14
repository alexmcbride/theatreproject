using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TheatreProject.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, StringLength(64)]
        public string Name { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public bool HasApprovedPosts
        {
            get { return Posts.Where(p => p.IsApproved).Any(); }
        }

        public int ApprovedPostCount
        {
            get { return Posts.Where(p => p.IsApproved).Count(); }
        }
    }
}