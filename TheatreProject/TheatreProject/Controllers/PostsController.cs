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
    [Authorize]
    public class PostsController : Controller
    {
        private const int MaxPostsPerPage = 6;

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        [AllowAnonymous]
        public ActionResult Index(int? category, int? page)
        {
            IQueryable<Post> posts = GetAllowedPosts();
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

            // Get posts.
            IQueryable<Post> posts = GetAllowedPosts(includeCategory: true, categoryId: id);
            var paginator = new Paginator<Post>(posts, page ?? 0, MaxPostsPerPage);
            return View(paginator);
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            Post post = GetAllowedPost(id, allowApproved: true);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(new PostDetailsViewModel
            {
                Post = post,
                Comments = GetCommentsForPost(post).ToList()
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public ActionResult CreateComment(int id, PostDetailsViewModel model)
        {
            Post post = GetAllowedPost(id, allowApproved: true);

            if (post == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                db.Comments.Add(new Comment
                {
                    PostId = post.PostId,
                    UserId = User.Identity.GetUserId(),
                    Content = model.Comment,
                    Posted = DateTime.Now,
                    IsApproved = true
                });
                db.SaveChanges();
                return RedirectToAction("details", new { id = post.PostId });
            }

            model.Post = post;
            model.Comments = GetCommentsForPost(post).ToList();
            return View(model);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult Create()
        {
            return View(new PostEditViewModel
            {
                Categories = new SelectList(db.Categories, "CategoryId", "Name")
            });
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CategoryId,Title,Content,IsApproved")] PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post();
                UpdateModel(post);

                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));
                post.Staff = (Staff)await userManager.FindByNameAsync(User.Identity.Name);
                post.Published = DateTime.Now;
                post.IsApproved = model.IsApproved && User.IsInRole("Admin"); // Only admin can set approved.

                db.Posts.Add(post);
                db.SaveChanges();

                // Redirect to details if approved, otherwise show approval needed warning.
                return RedirectToAction(post.IsApproved ? "details" : "approvalneeded", new { id = post.PostId });
            }

            model.Categories = new SelectList(db.Categories, "CategoryId", "Name", model.CategoryId);
            return View(model);
        }

        [Authorize(Roles = "Admin,Staff")]
        public ActionResult ApprovalNeeded(int id)
        {
            Post post = GetAllowedPost(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult Edit(int id)
        {
            Post post = GetAllowedPost(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(new PostEditViewModel
            {
                Content = post.Content,
                Title = post.Title,
                IsApproved = post.IsApproved && User.IsInRole("Admin"), // Only admin can set this.
                Categories = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId),
                CategoryId = post.CategoryId,
                Category = post.Category
            });
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "CategoryId,Title,IsApproved,Content")] PostEditViewModel model)
        {
            Post post = db.Posts.Find(id);

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
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult Delete(int id)
        {
            Post post = GetAllowedPost(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts
                .Include(p => p.Comments)
                .SingleOrDefault(p => p.PostId == id);

            // Remove any comments associated with this post.
            db.Comments.RemoveRange(post.Comments.ToList());

            // Save changes to 
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("index");
        }

        [AllowAnonymous]
        public ActionResult Staff(string id)
        {
            var staff = db.Users.Find(id) as Staff;
            if (staff == null)
            {
                return HttpNotFound();
            }

            // Get all of this member's posts.
            var posts = db.Posts
                .Where(p => p.StaffId == staff.Id && p.IsApproved)
                .OrderByDescending(p => p.Published);

            ViewBag.UserName = staff.UserName;

            return View(posts.ToList());
        }

        // Gets posts that this user can see.
        private IQueryable<Post> GetAllowedPosts()
        {
            return GetAllowedPosts(false, 0);
        }

        private IQueryable<Post> GetAllowedPosts(bool includeCategory, int categoryId)
        {
            IQueryable<Post> posts = db.Posts
                .Include(p => p.Staff)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Published);

            // Filter posts by category.
            if (includeCategory)
            {
                posts = posts.Where(p => p.CategoryId == categoryId);
            }

            // Admin can see all posts, so no filtering needed.
            if (!User.IsInRole("Admin"))
            {
                // Staff see approved posts and posts they've created.
                if (User.IsInRole("Staff"))
                {
                    string userId = User.Identity.GetUserId();
                    posts = posts.Where(p => p.IsApproved || p.StaffId == userId);
                }
                else
                {
                    // Everyone else sees only approved posts.
                    posts = posts.Where(p => p.IsApproved);
                }
            }

            return posts;
        }

        // Gets a single post if this user can access it.
        private Post GetAllowedPost(int id)
        {
            return GetAllowedPost(id, false);
        }

        private Post GetAllowedPost(int id, bool allowApproved)
        {
            var post = db.Posts
                .Include(p => p.Staff)
                .Include(p => p.Category)
                .SingleOrDefault(p => p.PostId == id);

            if (post != null)
            {
                // Allow all approved posts.
                if (allowApproved && post.IsApproved)
                {
                    return post;
                }

                // Admin always see posts.
                if (User.IsInRole("Admin"))
                {
                    return post;
                }

                // Staff can only view their own posts.
                string userId = User.Identity.GetUserId();
                if (User.IsInRole("Staff") && post.StaffId == userId)
                {
                    return post;
                }
            }

            return null;
        }

        private IOrderedQueryable<Comment> GetCommentsForPost(Post post)
        {
            return db.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == post.PostId)
                .OrderByDescending(c => c.Posted);
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
