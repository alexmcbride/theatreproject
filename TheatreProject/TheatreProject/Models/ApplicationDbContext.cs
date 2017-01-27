﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;

namespace TheatreProject.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public IQueryable<Staff> Staff
        {
            get { return Users.OfType<Staff>(); }
        }

        public IQueryable<Member> Members
        {
            get { return Users.OfType<Member>(); }
        }

        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            //Database.Initialize(true);
            Database.SetInitializer<ApplicationDbContext>(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        public Staff FindStaff(string id)
        {
            return (Staff)Users.Find(id);
        }

        public Member FindMember(string id)
        {
            return (Member)Users.Find(id);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // When user deleted, any comments associated with them are left, however the Comment.User property will be null
            // Comments are deleted when Post is deleted.
            modelBuilder.Entity<User>().HasOptional(u => u.Comments).WithMany().WillCascadeOnDelete(false);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}