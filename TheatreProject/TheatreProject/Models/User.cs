﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace TheatreProject.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class,
    // please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public abstract class User : IdentityUser
    {
        private ApplicationUserManager userManager;

        public DateTime Joined { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public string CurrentRole
        {
            get {
                if (userManager == null)
                {
                    userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                return userManager.GetRoles(Id).Single();
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}