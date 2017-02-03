﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;

namespace TheatreProject.Models
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            // Add roles.
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

            // Create admin.
            var staff = new Staff
            {
                UserName = "Admin",
                Email = "admin@admin.com",
                Joined = DateTime.Now,
                BirthDate = new DateTime(1970, 1, 1)
            };

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

                var result = userManager.Create(staff, "admin");
                if (result.Succeeded)
                {
                    // Give Admin correct roles.
                    userManager.AddToRole(staff.Id, "Member");
                    userManager.AddToRole(staff.Id, "Staff");
                    userManager.AddToRole(staff.Id, "Admin");
                }
                else
                {
                    throw new ApplicationException("Could not create user: " + result.Errors.FirstOrDefault());
                }
            }
        }
    }
}