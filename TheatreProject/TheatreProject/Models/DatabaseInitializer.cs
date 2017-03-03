using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace TheatreProject.Models
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        private Random random = new Random();

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            // Add roles if they don't exist.
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("Staff"));
            roleManager.Create(new IdentityRole("Member"));
            roleManager.Create(new IdentityRole("Suspended"));

            // Add some default categories.
            var news = context.Categories.Add(new Category { Name = "News" });
            var announcements = context.Categories.Add(new Category { Name = "Announcements" });
            var movies = context.Categories.Add(new Category { Name = "Movie Reviews" });
            var theatre = context.Categories.Add(new Category { Name = "Theatre Reviews" });
            context.SaveChanges();

            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));
            // Create admin if does not exist yet.
            if (userManager.FindByName("Admin") == null)
            {
                // Super liberal password validation for password for seeds
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
                var jeff = new Staff
                {
                    UserName = "Jeff",
                    Email = "jeff@localtheatrecompany.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(jeff, "staff");
                userManager.AddToRoles(jeff.Id, "Staff");
                var alex = new Staff
                {
                    UserName = "Xander",
                    Email = "xander@localtheatrecompany.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(alex, "staff");
                userManager.AddToRoles(alex.Id, "Staff");
                var paul = new Staff
                {
                    UserName = "Paul",
                    Email = "paul@localtheatrecompany.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(paul, "staff");
                userManager.AddToRoles(paul.Id, "Staff");

                // Create members
                var bob = new Member
                {
                    UserName = "Bob",
                    Email = "bob@gmail.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(bob, "member");
                userManager.AddToRoles(bob.Id, "Member");
                var steve = new Member
                {
                    UserName = "Steve",
                    Email = "steveb@gmail.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(steve, "member");
                userManager.AddToRoles(steve.Id, "Member");
                var gary = new Member
                {
                    UserName = "Gary",
                    Email = "gary@gmail.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(gary, "member");
                userManager.AddToRoles(gary.Id, "Member");
                var bill = new Member
                {
                    UserName = "Bill",
                    Email = "bill@gmail.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(bill, "member");
                userManager.AddToRoles(bill.Id, "Member");
                var greg = new Member
                {
                    UserName = "Greg",
                    Email = "greg@gmail.com",
                    Joined = DateTime.Now,
                    EmailConfirmed = true,
                };
                userManager.Create(greg, "member");
                userManager.AddToRoles(greg.Id, "Suspended");

                // some comments to use in posts.
                var comments = new List<Comment>
                {
                    new Comment { User=bill, Content="Pellentesque blandit mattis commodo. Proin tortor neque, pharetra in congue in, tempus nec neque. Curabitur fringilla blandit lectus, vel sodales erat euismod et. Curabitur volutpat euismod fringilla.", IsApproved=true, Posted=new DateTime(2017, 02, 16, 15, 12, 54 )},
                    new Comment { User=bob, Content="Nullam at porta libero, ac elementum odio. Morbi vestibulum justo et metus luctus gravida. Ut consequat feugiat rhoncus", IsApproved=true, Posted=new DateTime(2016, 10, 16, 19, 45, 43 )},
                    new Comment { User=bill, Content="Maecenas at placerat enim, quis mattis mauris. Nunc risus mi, semper et elit ut, accumsan viverra dui. Nulla condimentum, ipsum nec tempus consequat, ex urna vehicula lectus, at feugiat libero lectus quis ligula.", IsApproved=true, Posted=new DateTime(2017, 01, 12, 11, 53, 13 )},
                    new Comment { User=bob, Content="ras ac efficitur dui. Quisque molestie nulla ut nibh aliquet facilisis ac sed diam. Donec volutpat dolor eget ligula dignissim, sit amet vulputate leo porttitor. Suspendisse potenti.", IsApproved=true, Posted=new DateTime(2016, 03, 27, 03, 25, 24 )},
                    new Comment { User=bob, Content="Nullam eleifend ex urna, id tristique nisl vulputate non. In congue mauris ut lorem suscipit vestibulum non et quam. Vestibulum rhoncus semper dolor dignissim venenatis. Cras posuere ullamcorper est, nec cursus eros.", IsApproved=true, Posted=new DateTime(2016, 07, 04, 17, 42, 42 )},
                    new Comment { User=gary, Content="Ut molestie nunc at ultrices tincidunt. Nulla consequat efficitur hendrerit. Vestibulum urna eros, fringilla ut bibendum nec, cursus eget turpis. Maecenas tempus, tortor sed interdum cursus, sapien ligula commodo nisl, sed lacinia lectus nisi quis quam.", IsApproved=true, Posted=new DateTime(2015, 11, 30, 15, 14, 37 )},
                    new Comment { User=gary, Content="Maecenas laoreet porttitor dui ac ultricies. Pellentesque et suscipit velit, id finibus felis. Curabitur eu fermentum odio. Aenean venenatis eu eros vel vestibulum.", IsApproved=false, Posted=new DateTime(2016, 12, 15, 19, 44, 46 )},
                    new Comment { User=gary, Content="Nam nec risus ornare libero pellentesque fringilla. Vestibulum leo eros, bibendum vitae urna quis, varius mattis tortor. Aenean porta sagittis diam. Proin vitae mi magna.", IsApproved=true, Posted=new DateTime(2016, 05, 17, 22, 24, 51 )},
                    new Comment { User=gary, Content="Duis finibus commodo nibh, sed consectetur sapien luctus pharetra. Mauris at nisl commodo, tincidunt est non, aliquet leo. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.", IsApproved=true, Posted=new DateTime(2017, 01, 9, 9, 22, 42 )},
                    new Comment { User=bill, Content="Interdum et malesuada fames ac ante ipsum primis in faucibus. Sed in tellus ac nisi euismod pulvinar. Proin dignissim nulla lacus, eget euismod augue iaculis ac. Suspendisse faucibus non sem sit amet ultricies. Curabitur non tellus malesuada, hendrerit orci id, aliquet metus. Sed finibus justo sem, eget volutpat velit fermentum et. Duis lacinia enim interdum tortor elementum, non consequat purus pretium. ", IsApproved=true, Posted=new DateTime(2017, 02, 11, 13, 18, 52 )},
                };

                // Add some posts
                context.Posts.Add(new Post
                {
                    Category = news,
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean at suscipit eros, id consectetur nisl. Quisque aliquet gravida metus. Ut finibus dolor id augue congue tincidunt. Quisque sit amet sem quam. Etiam ultrices condimentum condimentum. Sed quis arcu viverra massa volutpat iaculis ut vel odio. Interdum et malesuada fames ac ante ipsum primis in faucibus. Curabitur convallis elementum pulvinar. Praesent est magna, gravida id tincidunt ut, accumsan ut elit. Pellentesque malesuada iaculis purus sed ultricies. Mauris in malesuada dolor. Morbi porta malesuada nibh, ut volutpat erat convallis sit amet. Nunc vulputate sodales purus ut elementum. Ut venenatis eros risus, vel pretium tortor efficitur in.",
                    IsApproved = true,
                    Published = new DateTime(2017, 02, 12, 13, 34, 13, 100),
                    Staff = jeff,
                    Title = "Lorem ipsum dolor sit amet",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = movies,
                    Content = "Fusce dignissim malesuada convallis. Mauris vel ante id augue fermentum tincidunt ut ut neque. Pellentesque sit amet convallis dolor. Phasellus euismod est quis mi fringilla, sit amet faucibus mauris finibus. Proin ultrices, risus non efficitur tristique, libero leo elementum mauris, ut iaculis neque dolor eleifend dolor. Etiam mauris est, tristique at interdum quis, maximus eu risus. Praesent feugiat tortor quam, eu scelerisque lacus pharetra vitae.",
                    IsApproved = true,
                    Published = new DateTime(2016, 12, 12, 14, 17, 11, 100),
                    Staff = paul,
                    Title = "Fusce dignissim malesuada convallis",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = theatre,
                    Content = "Duis eu quam erat. Integer et ipsum eu sem aliquet malesuada. Maecenas tristique, mi nec semper vestibulum, elit lectus auctor nulla, sed ullamcorper arcu velit at dui. Proin commodo massa ut lectus eleifend commodo. Aenean cursus ex et mauris ultricies mollis. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Praesent id cursus nisi, ultrices imperdiet justo. Mauris pharetra dui felis, vitae rutrum nulla accumsan quis. Nunc bibendum fringilla tortor, quis efficitur enim mattis nec. Proin aliquet id ex et euismod.",
                    IsApproved = true,
                    Published = new DateTime(2016, 11, 2, 13, 45, 15, 435),
                    Staff = paul,
                    Title = "Duis eu quam erat",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = news,
                    Content = "Suspendisse euismod mi et ipsum sagittis, et posuere diam fringilla. In hendrerit nulla et volutpat vestibulum. Donec a fermentum ex. Integer imperdiet tincidunt lacinia. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Fusce a sapien ut nisi molestie porttitor non et magna. Fusce eu est et nibh tincidunt accumsan sed quis massa. Phasellus vitae nulla tincidunt, fringilla sapien ac, gravida neque.",
                    IsApproved = true,
                    Published = new DateTime(2017, 02, 10, 10, 54, 46, 123),
                    Staff = admin,
                    Title = "Suspendisse euismod mi et ipsum sagittis",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = news,
                    Content = "In lorem neque, vulputate at tortor sed, viverra aliquam leo. Nam nec auctor odio. Aenean id interdum lacus. Curabitur pretium feugiat purus, in ullamcorper urna iaculis vitae. Integer lectus odio, pretium eu neque in, consequat fermentum sapien. Quisque sit amet tellus eget sem lacinia ultrices. Nullam consequat lacus non maximus semper. Aenean interdum felis sapien, quis consequat dui facilisis eu. Nulla ac lectus eu tellus consectetur fermentum. Proin et nibh eget nibh aliquet fringilla. Sed neque tellus, ullamcorper quis magna eu, porttitor molestie quam. Sed id laoreet nulla, vel efficitur dolor. Praesent non risus at sem sagittis aliquam. Ut efficitur est eget elit euismod aliquet. Curabitur ut ligula sed tortor ultricies finibus sed ut massa. Duis neque lacus, vulputate at pretium at, porta sit amet leo.",
                    IsApproved = true,
                    Published = new DateTime(2015, 05, 18, 17, 32, 35, 100),
                    Staff = jeff,
                    Title = "Vulputate at tortor sed",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = announcements,
                    Content = "Quisque a tincidunt augue. Curabitur tincidunt, lectus vitae tincidunt tempus, nibh dui ultricies mauris, in gravida enim metus sed elit. Maecenas purus nulla, dictum vitae consectetur eu, accumsan et mauris. Proin at quam non magna tincidunt gravida. Nullam bibendum, justo quis mattis tempus, elit nibh iaculis dolor, sit amet ultricies dolor felis eu libero. Etiam a volutpat enim. Mauris tincidunt nisl molestie, aliquam eros non, placerat risus.",
                    IsApproved = true,
                    Published = new DateTime(2016, 08, 12, 02, 37, 34, 143),
                    Staff = alex,
                    Title = "Quisque a tincidunt augue",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = announcements,
                    Content = "Phasellus cursus tempus ullamcorper. Fusce in neque viverra, tincidunt magna sit amet, tempor odio. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque efficitur massa vitae urna facilisis accumsan. Curabitur sit amet enim leo. Nam non lectus id metus iaculis finibus. Donec non nulla elit. Vestibulum dignissim orci sed consectetur pellentesque. Vestibulum eleifend egestas ipsum id sagittis. Duis viverra massa nec ultrices vestibulum. Aliquam mattis vehicula diam, eu porta orci dictum sed. Quisque et tempor orci, vitae convallis ex.",
                    IsApproved = false,
                    Published = new DateTime(2017, 02, 13, 13, 39, 00, 100),
                    Staff = alex,
                    Title = "Phasellus cursus tempus ullamcorper",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = movies,
                    Content = "Quisque vitae tellus ut libero dapibus pretium vestibulum et lacus. Pellentesque blandit urna nec lacinia iaculis. Ut eu nisi dapibus, scelerisque urna ac, pretium purus. Nunc sodales laoreet urna vel sagittis. Donec dictum, sem eget blandit sodales, enim felis luctus purus, et blandit est nisi sed neque. In at nunc quam. Aenean et bibendum enim. Sed sit amet sapien id mi gravida imperdiet facilisis id ligula. Nullam id finibus nunc. Donec quis dictum enim, eu viverra dolor. Cras gravida sem consequat, faucibus ex non, auctor quam. Maecenas ultrices at velit in facilisis. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Proin est metus, porttitor et libero eu, congue sagittis felis.",
                    IsApproved = true,
                    Published = new DateTime(2017, 02, 14, 11, 07, 23, 713),
                    Staff = jeff,
                    Title = "Quisque vitae tellus ut libero",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = theatre,
                    Content = "Nullam quis sapien lectus. Fusce id magna pharetra, molestie eros at, congue mauris. Nunc molestie dui id dictum semper. Integer et vulputate mauris. Nulla euismod id est vel sodales. Integer ultricies ullamcorper risus, sit amet blandit libero ornare ut. Nunc malesuada massa eget orci lacinia laoreet. Curabitur ac felis eget velit venenatis tincidunt eget eu urna. Maecenas semper tristique odio, eget consequat lectus feugiat at. Aliquam at lectus quam. Aenean a scelerisque odio. Proin fermentum dapibus felis vel rutrum. Duis blandit venenatis rhoncus. Nulla ac mauris ac odio euismod venenatis. Phasellus nulla ligula, semper a mattis non, malesuada at felis. Praesent sed mattis nulla, nec lobortis mauris.",
                    IsApproved = true,
                    Published = new DateTime(2015, 11, 27, 14, 07, 28, 317),
                    Staff = alex,
                    Title = "Nullam quis sapien lectus",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.Posts.Add(new Post
                {
                    Category = announcements,
                    Content = "Ut molestie nunc at ultrices tincidunt. Nulla consequat efficitur hendrerit. Vestibulum urna eros, fringilla ut bibendum nec, cursus eget turpis. Maecenas tempus, tortor sed interdum cursus, sapien ligula commodo nisl, sed lacinia lectus nisi quis quam. Maecenas laoreet porttitor dui ac ultricies. Pellentesque et suscipit velit, id finibus felis. Curabitur eu fermentum odio. Aenean venenatis eu eros vel vestibulum. Nam nec risus ornare libero pellentesque fringilla. Vestibulum leo eros, bibendum vitae urna quis, varius mattis tortor. Aenean porta sagittis diam. Proin vitae mi magna.",
                    IsApproved = true,
                    Published = new DateTime(2016, 02, 16, 11, 23, 28, 317),
                    Staff = alex,
                    Title = "Ut molestie nunc at ultrices tincidunt",
                    Comments = GetRandomSelectionOfComments(comments).ToList()
                });
                context.SaveChanges();

            }
        }

        private IEnumerable<Comment> GetRandomSelectionOfComments(IList<Comment> comments)
        {
            // Just grabs a random number of comments in a random order.
            int num = random.Next(0, comments.Count);
            return comments.OrderBy(c => Guid.NewGuid()).Take(num).Select(c => new Comment
            {
                CommentId = c.CommentId,
                Content = c.Content,
                IsApproved = c.IsApproved,
                Post = c.Post,
                Posted = c.Posted,
                PostId = c.PostId,
                User = c.User,
                UserId = c.UserId
            });
        }
    }
}