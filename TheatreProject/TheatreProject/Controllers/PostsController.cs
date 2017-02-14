using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TheatreProject.Helpers;
using TheatreProject.Models;
using TheatreProject.ViewModels;

namespace TheatreProject.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class PostsController : Controller
    {
        private const int MaxPostsPerPage = 6;

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        [AllowAnonymous]
        public ActionResult Index(int? page)
        {
            var posts = db.Posts
                .Include(p => p.Staff)
                .Include(p => p.Category)
                .Where(p => p.IsApproved)
                .OrderByDescending(p => p.Published);

            var paginator = new Paginator<Post>(posts, page ?? 0, MaxPostsPerPage);

            return View(paginator);
        }

        // GET: Posts/Category/5
        [AllowAnonymous]
        public ActionResult Category(int id, int? page)
        {
            // Get category
            var category = db.Categories.SingleOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = category.CategoryId;
            ViewBag.Category = category.Name;

            // Get posts (incuding staff members)
            var posts = db.Posts
                .Include(p => p.Staff)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id && p.IsApproved)
                .OrderByDescending(p => p.Published);

            var paginator = new Paginator<Post>(posts, page ?? 0, MaxPostsPerPage);

            return View(paginator);
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = db.Posts
                .Include(p => p.Staff)
                .Include(p => p.Category)
                .SingleOrDefault(p => p.PostId == id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CategoryId,Title,Content")] Post post)
        {
            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));
            post.Staff = (Staff)await userManager.FindByNameAsync(User.Identity.Name);
            post.Published = DateTime.Now;
            post.IsApproved = false;

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("details", new { id = post.PostId });
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int id)
        {
            Post post = db.Posts
                .Include(p => p.Category)
                .SingleOrDefault(p => p.PostId == id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(new PostEditViewModel
            {
                Content = post.Content,
                Title = post.Title,
                IsApproved = post.IsApproved,
                Categories = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId),
                CategoryId = post.CategoryId,
                Category = post.Category
            });
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "CategoryId,Title,IsApproved,Content")] PostEditViewModel model)
        {
            Post post = db.Posts
                .Include(p => p.Category)
                .SingleOrDefault(p => p.PostId == id);

            if (post == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                UpdateModel(post);
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("details", new { id = post.PostId });
            }

            model.Category = post.Category;
            model.Categories = new SelectList(db.Categories, "CategoryId", "Name", model.CategoryId);

            return View(model);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("index");
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
