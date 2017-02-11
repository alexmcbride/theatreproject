using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheatreProject.Models;
using TheatreProject.ViewModels;

namespace TheatreProject.Controllers
{
    // This controller inherits from AccountController so we can borrow login/registration stuff.
    [Authorize(Roles = "Admin")]
    public class UsersController : AccountController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public UsersController() : base() { }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager) { }

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
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
        public async Task<ActionResult> Create(CreateStaffViewModel model)
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

                    return RedirectToAction("index");
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
                UserName = staff.UserName
            });
        }

        // POST: Users/EditStaff/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditStaff(string id, EditStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                Staff staff = (Staff)await UserManager.FindByIdAsync(id);
                UpdateModel(staff);

                IdentityResult result = await UserManager.UpdateAsync(staff);
                if (result.Succeeded)
                {
                    return RedirectToAction("index");
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
                IsSuspended = member.IsSuspended
            });
        }

        // POST: Users/EditMember/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMember(string id, EditMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                Member member = (Member)await UserManager.FindByIdAsync(id);
                UpdateModel(member);

                IdentityResult result = await UserManager.UpdateAsync(member);
                if (result.Succeeded)
                {
                    return RedirectToAction("index");
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
                return View("DeleteOwnAccountError");
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
            await UserManager.DeleteAsync(user);
            return RedirectToAction("index");
        }

        public async Task<ActionResult> ChangeRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == User.Identity.GetUserId())
            {
                return View("ChangeOwnRoleError");
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
                Roles = items
            });
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, ChangeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByIdAsync(id);

                // Change user's role.
                string oldRole = (await UserManager.GetRolesAsync(id)).Single();
                await UserManager.RemoveFromRoleAsync(id, oldRole);
                await UserManager.AddToRoleAsync(id, model.Role);

                // Info for confirmation view.
                ViewBag.OldRole = oldRole;
                ViewBag.NewRole = model.Role;
                ViewBag.UserName = user.UserName;

                return View("ChangeRoleConfirmed");                
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
