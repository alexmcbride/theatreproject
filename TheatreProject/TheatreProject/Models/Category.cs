using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheatreProject.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}