using System;
using System.Collections.Generic;

namespace TheatreProject.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public DateTime Published { get; set; }
        public bool IsApproved { get; set; }
        public string Content { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Staff User { get; set; }
        public virtual Category Category { get; set; }
    }
}