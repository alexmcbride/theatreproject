using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheatreProject.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public DateTime Posted { get; set; }
        public bool IsApproved { get; set; }
        public string Content { get; set; }

        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}