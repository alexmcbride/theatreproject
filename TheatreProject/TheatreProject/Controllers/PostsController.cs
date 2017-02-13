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
                .OrderByDescending(p => p.Published)
                .Include(p => p.Staff)
                .Include(p => p.Category);

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
                .Where(p => p.CategoryId == id)
                .Include(p => p.Staff)
                .Include(p => p.Category)
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
            post.Staff = (Staff)await userManager.FindByIdAsync(User.Identity.GetUserId());
            post.Published = DateTime.Now;
            post.IsApproved = false;

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = post.PostId });
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,CategoryId,StaffId,Title,Published,IsApproved,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);
            return View(post);
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
            return RedirectToAction("Index");
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
