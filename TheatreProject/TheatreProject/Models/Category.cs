using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public bool HasApprovedPosts
        {
            get { return Posts != null && Posts.Where(p => p.IsApproved).Any(); }
        }

        [NotMapped]
        public int ApprovedPostCount
        {
            get { return Posts == null ? 0 : Posts.Where(p => p.IsApproved).Count(); }
        }
    }
}