using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;


namespace TheatreProject.Models
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            // Add roles if they don't exist.
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }
            if (!roleManager.RoleExists("Staff"))
            {
                roleManager.Create(new IdentityRole("Staff"));
            }
            if (!roleManager.RoleExists("Member"))
            {
                roleManager.Create(new IdentityRole("Member"));
            }

            // Add some default categories.
            var category = context.Categories.Add(new Category { Name = "News" });
            context.Categories.Add(new Category { Name = "Announcements" });
            context.Categories.Add(new Category { Name = "Movie Reviews" });
            context.Categories.Add(new Category { Name = "Theatre Reviews" });
            context.SaveChanges();

            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));
            // Create admin if does not exist yet.
            if (userManager.FindByName("Admin") == null)
            {
                // Super liberal password validation for password "admin"
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequireDigit = false,
                    RequiredLength = 3,
                    RequireLowercase = false,
                    RequireNonLetterOrDigit = false,
                    RequireUppercase = false,
                };

                // Create admin.
                var admin = new Staff
                {
                    UserName = "Admin",
                    Email = "admin@admin.com",
                    Joined = DateTime.Now,
                };
                userManager.Create(admin, "admin");
                userManager.AddToRoles(admin.Id, "Admin");

                // Create staff.
                var staff = new Staff
                {
                    UserName = "Staff",
                    Email = "staff@staff.com",
                    Joined = DateTime.Now,
                };
                userManager.Create(staff, "staff");
                userManager.AddToRoles(staff.Id, "Staff");

                // add some posts
                var post = new Post
                {
                    Category = category,
                    Content = "Test Content",
                    IsApproved = true,
                    Published = DateTime.Now,
                    Staff = staff,
                    Title = "Test Title",
                };
                context.Posts.Add(post);
                context.SaveChanges();

                // Create member.
                var member = new Member
                {
                    UserName = "Member",
                    Email = "member@member.com",
                    Joined = DateTime.Now
                };

                userManager.Create(member, "member");
                userManager.AddToRoles(member.Id, "Member");
            }
        }
    }
}