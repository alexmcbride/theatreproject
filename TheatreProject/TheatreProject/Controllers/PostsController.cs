using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TheatreProject.Helpers;
using TheatreProject.Models;
using TheatreProject.ViewModels;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;
using System.ServiceModel.Syndication;
using System.Collections.Generic;
using System.Xml;

namespace TheatreProject.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private const int MaxPostsPerPage = 5;

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        [AllowAnonymous]
        public ActionResult Index(int? page)
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
                    IsApproved = false
                });
                db.SaveChanges();

                Flash.Instance.Success("Approval", "Your comment needs to be approved by an admin before it can be viewed");

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
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryId,Title,Content")] PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Update post from model state.
                Post post = new Post();
                UpdateModel(post);

                // Set our properties.
                post.StaffId = User.Identity.GetUserId();
                post.Published = DateTime.Now;
                post.IsApproved = false;

                // Add new post to database.
                db.Posts.Add(post);
                db.SaveChanges();

                Flash.Instance.Warning("Approval Needed", string.Format("The post '{0}' will need to be approved before it is displayed", post.Title));

                return RedirectToAction("details", new { id = post.PostId });
            }

            model.Categories = new SelectList(db.Categories, "CategoryId", "Name", model.CategoryId);
            return View(model);
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
        public ActionResult Edit(int id, [Bind(Include = "CategoryId,Title,Content")] PostEditViewModel model)
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

                Flash.Instance.Success("Edited", string.Format("The post '{0}' has been edited", post.Title));

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

            string title = post.Title;

            // Remove any comments associated with this post.
            db.Comments.RemoveRange(post.Comments.ToList());

            // Save changes to 
            db.Posts.Remove(post);
            db.SaveChanges();

            Flash.Instance.Success("Deleted", string.Format("The post '{0}' has been successfully deleted", title));

            return RedirectToAction("index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Approve(int id)
        {
            return UpdatePostApproval(id, approved: true);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Disallow(int id)
        {
            return UpdatePostApproval(id, approved: false);
        }

        private ActionResult UpdatePostApproval(int id, bool approved)
        {
            Post post = db.Posts.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            if (post.IsApproved != approved)
            {
                post.IsApproved = approved;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (post.IsApproved)
            {
                Flash.Instance.Success("Approved", string.Format("The post '{0}' has been approved", post.Title));
            }
            else
            {
                Flash.Instance.Success("Disallowed", string.Format("The post '{0}' has been disallowed", post.Title));
            }

            return RedirectToAction("details", "posts", new { id = post.PostId });
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

        private IQueryable<Comment> GetCommentsForPost(Post post)
        {
            IQueryable<Comment> comments = db.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == post.PostId)
                .OrderByDescending(c => c.Posted);

            if (User.Identity.IsAuthenticated)
            {
                // Admin see all comments.
                if (!User.IsInRole("Admin"))
                {
                    // other users see only approved comments and their own.
                    string userId = User.Identity.GetUserId();
                    comments = comments.Where(p => p.IsApproved || p.UserId == userId);
                }
            }
            else
            {
                // Non-members see only approved posts.
                comments = comments.Where(p => p.IsApproved);
            }

            return comments;
        }

        //public ActionResult Rss()
        //{
        //    IQueryable<Post> posts = db.Posts
        //        .Include(p => p.Staff)
        //        .Where(p => p.IsApproved)
        //        .Take(10)
        //        .OrderByDescending(p => p.Published);

        //    var items = new List<SyndicationItem>();
        //    foreach (var post in posts)
        //    {
        //        SyndicationItem item = new SyndicationItem();
        //        item.Title = new TextSyndicationContent(post.Title);
        //        item.Authors.Add(new SyndicationPerson(post.Staff.Email));
        //        item.Id = post.PostId.ToString();
        //        item.PublishDate = post.Published;
        //        item.Summary = new TextSyndicationContent(post.Content.Substring(100));
                
        //        items.Add(item);

       
        //    }

        //    SyndicationFeed feed = new SyndicationFeed(items);
        //    feed.Title = new TextSyndicationContent("Local Theatre Company");
        //    feed.Description = new TextSyndicationContent("A blog about local theatre");

        //    return View();
        //}

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
