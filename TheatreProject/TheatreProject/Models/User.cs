using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TheatreProject.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class,
    // please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public abstract class User : IdentityUser
    {
        public DateTime Joined { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public string MemberType {
            get
            {
                if (this is Member)
                {
                    return "Member";
                }

                Staff staff = this as Staff;
                if (staff != null)
                {
                    if (staff.IsAdmin)
                    {
                        return "Admin";
                    }

                    return "Staff";
                }

                return "None";
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