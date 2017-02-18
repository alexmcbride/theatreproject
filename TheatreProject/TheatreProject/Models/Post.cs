﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TheatreProject.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [ForeignKey("Category"), Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("Staff"), Display(Name = "Staff")]
        public string StaffId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime Published { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual Category Category { get; set; }

        public bool HasApproveComments
        {
            get { return Comments.Any(c => c.IsApproved); }
        }

        public int ApproveCommentCount
        {
            get { return Comments.Where(c => c.IsApproved).Count(); }
        }

        public bool BelongsTo(string userId)
        {
            return StaffId == userId;
        }
    }
}