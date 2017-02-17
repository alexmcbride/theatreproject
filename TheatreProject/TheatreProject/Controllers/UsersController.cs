using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheatreProject.Helpers;
using TheatreProject.Models;
using TheatreProject.ViewModels;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;

namespace TheatreProject.Controllers
{
    // Controller inherits from AccountController so we can borrow login/registration stuff.
    [Authorize(Roles = "Admin")]
    public class UsersController : AccountController
    {
        private const int MaxUsersPerPage = 20;
        private ApplicationDbContext db = new ApplicationDbContext();

        public UsersController() : base() { }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager) { }

        // GET: Users
        public ActionResult Index(int? page)
        {
            var users = db.Users.OrderBy(u => u.Joined);
            var paginator = new Paginator<User>(users, page ?? 0, MaxUsersPerPage);

            return View(paginator);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (user is Staff)
            {
                return View("DetailsStaff", (Staff)user);
            }

            if (user is Member)
            {
                return View("DetailsMember", (Member)user);
            }

            return HttpNotFound();
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserName,Email,PhoneNumber,FirstName,LastName,Address,City,PostCode,BirthDate,Password,PasswordConfirm,EmailConfirmed")] CreateStaffViewModel model)
        {
            // Add new staff member to system.
            if (ModelState.IsValid)
            {
                Staff staff = new Staff();
                UpdateModel(staff);
                staff.Joined = DateTime.Now;

                IdentityResult result = await UserManager.CreateAsync(staff, model.Password);
                if (result.Succeeded)
                {
                    // Assign roles to this staff member.
                    await UserManager.AddToRolesAsync(staff.Id, "Staff");

                    Flash.Instance.Success("Created", "The user has been created");

                    return RedirectToAction("index", "users");
                }
                else
                {
                    AddErrors(result);
                }
            }

            return View(model);
        }

        // GET: Users/EditStaff/5
        public ActionResult EditStaff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Staff staff = db.Users.Find(id) as Staff;

            if (staff == null)
            {
                return HttpNotFound();
            }

            return View(new EditStaffViewModel
            {
                Address = staff.Address,
                BirthDate = staff.BirthDate ?? new DateTime(1900, 1, 1),
                City = staff.City,
                Email = staff.Email,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                PhoneNumber = staff.PhoneNumber,
                PostCode = staff.PostCode,
                UserName = staff.UserName,
                EmailConfirmed = staff.EmailConfirmed
            });
        }

        // POST: Users/EditStaff/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditStaff(string id, [Bind(Include = "UserName,Email,PhoneNumber,FirstName,LastName,Address,City,PostCode,BirthDate,EmailConfirmed")]EditStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                Staff staff = (Staff)await UserManager.FindByIdAsync(id);
                UpdateModel(staff);

                IdentityResult result = await UserManager.UpdateAsync(staff);
                if (result.Succeeded)
                {
                    Flash.Instance.Success("Edited", "The staff member has been edited");
                    return RedirectToAction("index", "users");
                }
                AddErrors(result);
            }

            return View(model);
        }

        // GET: Users/EditMember/5
        public async Task<ActionResult> EditMember(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Member member = await UserManager.FindByIdAsync(id) as Member;

            if (member == null)
            {
                return HttpNotFound();
            }

            return View(new EditMemberViewModel
            {
                Email = member.Email,
                UserName = member.UserName,
                EmailConfirmed = member.EmailConfirmed
            });
        }

        // POST: Users/EditMember/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMember(string id, [Bind(Include = "UserName,Email,IsSuspended,EmailConfirmed")] EditMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                Member member = (Member)await UserManager.FindByIdAsync(id);
                UpdateModel(member);

                IdentityResult result = await UserManager.UpdateAsync(member);
                if (result.Succeeded)
                {
                    Flash.Instance.Success("Edited", "The member has been edited");
                    return RedirectToAction("index", "users");
                }
                AddErrors(result);
            }
            return View(model);
        }


        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == User.Identity.GetUserId())
            {
                Flash.Instance.Info("Delete Error", "You cannot delete your own account");
                return RedirectToAction("index", "users");
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            User user = await UserManager.FindByIdAsync(id);
            bool save = false;

            // Remove any posts owned by this user.
            Staff staff = user as Staff;
            if (staff != null)
            {
                var posts = db.Posts.Where(p => p.StaffId == staff.Id).ToList();
                db.Posts.RemoveRange(posts);
                save = true;
            }

            // Delete comments owned by this user.
            var comments = db.Comments.Where(c => c.UserId == user.Id).ToList();
            if (comments.Any())
            {
                db.Comments.RemoveRange(comments);
                save = true;
            }

            // Save if needed
            if (save)
            {
                await db.SaveChangesAsync();
            }

            // Delete user.
            await UserManager.DeleteAsync(user);

            Flash.Instance.Success("Deleted", "The user has been deleted");
            return RedirectToAction("index", "users");
        }

        // GET: Users/ChangeRole/5
        public async Task<ActionResult> ChangeRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Can't change your own role.
            if (id == User.Identity.GetUserId())
            {
                Flash.Instance.Info("Change Role", "You cannot change your own role");
                return RedirectToAction("index", "users");
            }

            User user = await UserManager.FindByIdAsync(id);
            string role = (await UserManager.GetRolesAsync(id)).Single(); // Only ever a single role.

            var items = db.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == role
            }).ToList();

            return View(new ChangeRoleViewModel
            {
                UserName = user.UserName,
                Roles = items
            });
        }

        // POST: Users/ChangeRole/5
        [HttpPost, ValidateAntiForgeryToken, ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, [Bind(Include = "Role")] ChangeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByIdAsync(id);
                string oldRole = (await UserManager.GetRolesAsync(id)).Single(); // Only ever a single role.

                if (oldRole == model.Role)
                {
                    // User already has role, so don't change it.
                    Flash.Instance.Info("Role Error", string.Format("The user {0} already has the role {1}", user.UserName, model.Role));
                    return RedirectToAction("index", "users");
                }
                else
                {
                    // Remove old role and add new one.
                    await UserManager.RemoveFromRoleAsync(id, oldRole);
                    await UserManager.AddToRoleAsync(id, model.Role);

                    // If role is anything other than suspended we need to change type.
                    if (model.Role != "Suspended")
                    {
                        // Update discriminator to change the type of this user. This is a bit of a hack, but it works!
                        db.Database.ExecuteSqlCommand(
                            "UPDATE AspNetUsers SET Discriminator={0} WHERE id={1}",
                            model.Role == "Admin" ? "Staff" : model.Role,
                            id);
                    }

                    // Redirect after change to make sure new user type is loaded.
                    Flash.Instance.Success(
                        "Role Changed",
                        string.Format("The user {0}'s role changed to {1}", user.UserName, model.Role));
                    return RedirectToAction("index", "users");
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
