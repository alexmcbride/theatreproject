using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;


namespace TheatreProject.Models
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
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
            var news = context.Categories.Add(new Category { Name = "News" });
            var announcements = context.Categories.Add(new Category { Name = "Announcements" });
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
                    EmailConfirmed = true,
                };
                userManager.Create(admin, "admin");
                userManager.AddToRoles(admin.Id, "Admin");

                // Create staff.
                var staff = new Staff
                {
                    UserName = "Staff",
                    Email = "staff@staff.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(staff, "staff");
                userManager.AddToRoles(staff.Id, "Staff");

                // Create member.
                var member = new Member
                {
                    UserName = "Member",
                    Email = "member@member.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };

                userManager.Create(member, "member");
                userManager.AddToRoles(member.Id, "Member");

                // Add some posts
                context.Posts.Add(new Post
                {
                    Category = news,
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean at suscipit eros, id consectetur nisl. Quisque aliquet gravida metus. Ut finibus dolor id augue congue tincidunt. Quisque sit amet sem quam. Etiam ultrices condimentum condimentum. Sed quis arcu viverra massa volutpat iaculis ut vel odio. Interdum et malesuada fames ac ante ipsum primis in faucibus. Curabitur convallis elementum pulvinar. Praesent est magna, gravida id tincidunt ut, accumsan ut elit. Pellentesque malesuada iaculis purus sed ultricies. Mauris in malesuada dolor. Morbi porta malesuada nibh, ut volutpat erat convallis sit amet. Nunc vulputate sodales purus ut elementum. Ut venenatis eros risus, vel pretium tortor efficitur in.",
                    IsApproved = true,
                    Published = new DateTime(2017, 02, 12, 13, 34, 13, 100),
                    Staff = staff,
                    Title = "Lorem ipsum dolor sit amet",
                });
                context.Posts.Add(new Post
                {
                    Category = news,
                    Content = "Suspendisse euismod mi et ipsum sagittis, et posuere diam fringilla. In hendrerit nulla et volutpat vestibulum. Donec a fermentum ex. Integer imperdiet tincidunt lacinia. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Fusce a sapien ut nisi molestie porttitor non et magna. Fusce eu est et nibh tincidunt accumsan sed quis massa. Phasellus vitae nulla tincidunt, fringilla sapien ac, gravida neque.",
                    IsApproved = true,
                    Published = new DateTime(2017, 02, 10, 10, 54, 46, 123),
                    Staff = admin,
                    Title = "Suspendisse euismod mi et ipsum sagittis",
                });
                context.Posts.Add(new Post
                {
                    Category = news,
                    Content = "In lorem neque, vulputate at tortor sed, viverra aliquam leo. Nam nec auctor odio. Aenean id interdum lacus. Curabitur pretium feugiat purus, in ullamcorper urna iaculis vitae. Integer lectus odio, pretium eu neque in, consequat fermentum sapien. Quisque sit amet tellus eget sem lacinia ultrices. Nullam consequat lacus non maximus semper. Aenean interdum felis sapien, quis consequat dui facilisis eu. Nulla ac lectus eu tellus consectetur fermentum. Proin et nibh eget nibh aliquet fringilla. Sed neque tellus, ullamcorper quis magna eu, porttitor molestie quam. Sed id laoreet nulla, vel efficitur dolor. Praesent non risus at sem sagittis aliquam. Ut efficitur est eget elit euismod aliquet. Curabitur ut ligula sed tortor ultricies finibus sed ut massa. Duis neque lacus, vulputate at pretium at, porta sit amet leo.",
                    IsApproved = true,
                    Published = new DateTime(2015, 05, 18, 17, 32, 35, 100),
                    Staff = staff,
                    Title = "Vulputate at tortor sed",
                });
                context.Posts.Add(new Post
                {
                    Category = announcements,
                    Content = "Phasellus cursus tempus ullamcorper. Fusce in neque viverra, tincidunt magna sit amet, tempor odio. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque efficitur massa vitae urna facilisis accumsan. Curabitur sit amet enim leo. Nam non lectus id metus iaculis finibus. Donec non nulla elit. Vestibulum dignissim orci sed consectetur pellentesque. Vestibulum eleifend egestas ipsum id sagittis. Duis viverra massa nec ultrices vestibulum. Aliquam mattis vehicula diam, eu porta orci dictum sed. Quisque et tempor orci, vitae convallis ex.",
                    IsApproved = true,
                    Published = new DateTime(2017, 02, 13, 13, 39, 00, 100),
                    Staff = admin,
                    Title = "Phasellus cursus tempus ullamcorper",
                });
                context.SaveChanges();
            }
        }
    }
}