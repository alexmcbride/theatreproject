using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TheatreProject.Models;
using TheatreProject.ViewModels;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;

namespace TheatreProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Post).Include(c => c.User);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get comment, post, and user.
            Comment comment = db.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .SingleOrDefault(c => c.CommentId == id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(new CommentEditViewModel
            {
                Content = comment.Content,
                UserName = comment.User.UserName,
                PostId = comment.PostId,
                PostTitle = comment.Post.Title,
                CategoryId = comment.Post.CategoryId,
                CategoryName = comment.Post.Category.Name
            });
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, [Bind(Include = "Content")] CommentEditViewModel model)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get comment, post, and user.
            Comment comment = db.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .SingleOrDefault(c => c.CommentId == id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                comment.Content = model.Content;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();

                Flash.Instance.Success("Comment Edited", "The comment has been edited");
                return RedirectToAction("details", "posts", new { id = comment.PostId });
            }

            model.CategoryId = comment.Post.CategoryId;
            model.CategoryName = comment.Post.Category.Name;
            model.PostTitle = comment.Post.Title;
            model.PostId = comment.PostId;
            return View(model);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
