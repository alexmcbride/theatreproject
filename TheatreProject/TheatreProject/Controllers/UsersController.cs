using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheatreProject.Helpers;
using TheatreProject.Models;
using TheatreProject.ViewModels;

namespace TheatreProject.Controllers
{
    // This controller inherits from AccountController so we can borrow login/registration stuff.
    [Authorize(Roles = "Admin")]
    public class UsersController : AccountController
    {
        private const int MaxUsersPerPage = 50;
        private ApplicationDbContext db = new ApplicationDbContext();

        public UsersController() : base() { }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager) { }

        // GET: Users
        public ActionResult Index(int? page, UsersMessageId? message)
        {
            UpdateMessage(message);

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

                    return RedirectToAction("index", "users", new { message = UsersMessageId.Added });
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
                    return RedirectToAction("index", "users", new { message = UsersMessageId.Edited });
                }
                AddErrors(result);
            }

            return View(model);
        }

        // GET: Users/EditMember/5
        public ActionResult EditMember(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Member member = db.Users.Find(id) as Member;

            if (member == null)
            {
                return HttpNotFound();
            }

            return View(new EditMemberViewModel
            {
                Email = member.Email,
                UserName = member.UserName,
                IsSuspended = member.IsSuspended ?? false,
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
                    return RedirectToAction("index", "users", new { message = UsersMessageId.Edited });
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
                return RedirectToAction("index", "users", new { message = UsersMessageId.DeleteError });
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

            // Remove any posts owned by this user.
            Staff staff = user as Staff;
            if (staff != null)
            {
                var posts = db.Posts.Where(p => p.StaffId == staff.Id).ToList();
                db.Posts.RemoveRange(posts);
            }

            // Delete comments owned by this user.
            var comments = db.Comments.Where(c => c.UserId == user.Id).ToList();
            db.Comments.RemoveRange(comments);
            await db.SaveChangesAsync();

            // Delete user.
            await UserManager.DeleteAsync(user);
            return RedirectToAction("index", "users", new { message = UsersMessageId.Deleted });
        }

        // GET: Users/ChangeRole/5
        public async Task<ActionResult> ChangeRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (User.Identity.GetUserId() == null || id == User.Identity.GetUserId())
            {
                return RedirectToAction("index", "users", new { message = UsersMessageId.RoleChangeError });
            }

            User user = await UserManager.FindByIdAsync(id);
            string role = (await UserManager.GetRolesAsync(id)).Single();

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
                // Remove old role and add new one.
                User user = await UserManager.FindByIdAsync(id);
                string oldRole = (await UserManager.GetRolesAsync(id)).Single();

                if (oldRole != model.Role)
                {
                    await UserManager.RemoveFromRoleAsync(id, oldRole);
                    await UserManager.AddToRoleAsync(id, model.Role);

                    // Update discriminator to change the type of this user. This is a bit of a hack, but it works!
                    db.Database.ExecuteSqlCommand(
                        "UPDATE AspNetUsers SET Discriminator={0} WHERE id={1}",
                        model.Role == "Admin" ? "Staff" : model.Role,
                        id);
                }

                // Redirect after change to make sure new user type is loaded.
                return RedirectToAction("index", "users", new { id = id, oldRole = oldRole, message = UsersMessageId.RoleNotChanged });
            }

            return View(model);
        }

        public enum UsersMessageId
        {
            None,
            Added,
            Edited,
            Deleted,
            RoleChanged,
            RoleChangeError,
            RoleNotChanged,
            DeleteError,
        }

        private void UpdateMessage(UsersMessageId? message)
        {
            switch (message ?? UsersMessageId.None)
            {
                case UsersMessageId.Added:
                    ViewData["Message"] = "A new user has been added";
                    ViewData["MessageType"] = "success";
                    break;
                case UsersMessageId.Edited:
                    ViewData["Message"] = "The user has been edited";
                    ViewData["MessageType"] = "success";
                    break;
                case UsersMessageId.Deleted:
                    ViewData["Message"] = "The user has been deleted";
                    ViewData["MessageType"] = "success";
                    break;
                case UsersMessageId.RoleChanged:
                    ViewData["Message"] = "The user's role has been changed";
                    ViewData["MessageType"] = "success";
                    break;
                case UsersMessageId.RoleChangeError:
                    ViewData["Message"] = "Your cannot change your own role";
                    ViewData["MessageType"] = "warning";
                    break;
                case UsersMessageId.RoleNotChanged:
                    ViewData["Message"] = "The user already has this role";
                    ViewData["MessageType"] = "warning";
                    break;
                case UsersMessageId.DeleteError:
                    ViewData["Message"] = "You cannot delete your own account";
                    ViewData["MessageType"] = "warning";
                    break;
            }
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
