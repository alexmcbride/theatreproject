using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace TheatreProject.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {

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