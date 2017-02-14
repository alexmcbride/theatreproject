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

        // Gets posts that the user can see.
        private IQueryable<Post> GetPostsForUser(bool includeCategory = false, int categoryId = 0)
        {
            IQueryable<Post> posts = db.Posts
                .Include(p => p.Staff)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Published);

            // If category is needed then include that.
            if (includeCategory)
            {
                posts = posts.Where(p => p.CategoryId == categoryId);
            }

            // Get approved posts and any posts that belong to the user, or get all if user is an admin.
            if (User.Identity.IsAuthenticated)
            {
                bool admin = User.IsInRole("Admin");
                User user = db.Users.Single(u => u.UserName == User.Identity.Name);
                posts = posts.Where(p => admin || p.IsApproved || p.StaffId == user.Id);
            }
            else
            {
                posts = posts.Where(p => p.IsApproved);
            }

            return posts;
        }

        // Gets post for a single user, if they can view it.
        private Post GetPostForUser(int id, bool allowApproved = false)
        {
            var post = db.Posts
                .Include(p => p.Staff)
                .Include(p => p.Category)
                .SingleOrDefault(p => p.PostId == id);

            if (post != null)
            {
                // Everyone can see approved posts.
                if (allowApproved && post.IsApproved)
                {
                    return post;
                }

                if (User.Identity.IsAuthenticated)
                {
                    // Admin always see posts.
                    if (User.IsInRole("Admin"))
                    {
                        return post;
                    }

                    // Users can see posts if they belong to them.
                    User user = db.Users.Single(u => u.UserName == User.Identity.Name);
                    if (post.StaffId == user.Id)
                    {
                        return post;
                    }
                }
            }

            return null;
        }

        // GET: Posts
        [AllowAnonymous]
        public ActionResult Index(int? page)
        {
            var posts = GetPostsForUser();
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
            var posts = GetPostsForUser(includeCategory: true, categoryId: id);
            var paginator = new Paginator<Post>(posts, page ?? 0, MaxPostsPerPage);
            return View(paginator);
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            Post post = GetPostForUser(id, allowApproved: true);
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
            Post post = GetPostForUser(id, allowApproved: true);
            if (post == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));
                db.Comments.Add(new Comment
                {
                    Post = post,
                    User = userManager.FindByName(User.Identity.Name),
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

        private IOrderedQueryable<Comment> GetCommentsForPost(Post post)
        {
            return db.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == post.PostId)
                .OrderByDescending(c => c.Posted);
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
        public async Task<ActionResult> Create([Bind(Include = "CategoryId,Title,Content")] PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Post post = new Post();
                UpdateModel(post);

                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(db));
                post.Staff = (Staff)await userManager.FindByNameAsync(User.Identity.Name);
                post.Published = DateTime.Now;
                post.IsApproved = false;

                db.Posts.Add(post);
                db.SaveChanges();

                return RedirectToAction("details", new { id = post.PostId });
            }

            model.Categories = new SelectList(db.Categories, "CategoryId", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin,Staff")]
        public ActionResult Edit(int id)
        {
            Post post = GetPostForUser(id);

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
            Post post = GetPostForUser(id);
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
