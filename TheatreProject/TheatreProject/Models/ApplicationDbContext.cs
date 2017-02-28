using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;

namespace TheatreProject.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new DatabaseInitializer());

            // Log SQL queries to the debug output.
            //Database.Log = s => Debug.WriteLine(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set cascade on delete for foreign key relationships.
            modelBuilder.Entity<User>().HasOptional(u => u.Comments).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Staff>().HasOptional(u => u.Posts).WithMany().WillCascadeOnDelete(true);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}