using System;

namespace TheatreProject.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime Posted { get; set; }
        public bool IsApproved { get; set; }
        public string Content { get; set; }

        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}