using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TheatreProject.Models;
using TheatreProject.ViewModels;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;
using TheatreProject.Helpers;

namespace TheatreProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "Content")] CommentEditViewModel model)
        {
            // Get comment, post, and user.
            Comment comment = db.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .SingleOrDefault(c => c.CommentId == id);

            if (ModelState.IsValid)
            {
                comment.Content = model.Content;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();

                Flash.Instance.Success("Comment Edited", "The comment has been edited");
                return RedirectToAction("details", "posts", new { id = comment.PostId });
            }

            // Get stuff for breadcrumb.
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

            Comment comment = db.Comments
                .Include(c => c.Post)
                .Include(c => c.User)
                .SingleOrDefault(c => c.CommentId == id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(new CommentDeleteViewModel
            {
                UserName = comment.User.UserName,
                Posted = comment.Posted,
                Email = comment.User.Email,
                PostId = comment.PostId,
                PostTitle = comment.Post.Title,
                CategoryId = comment.Post.CategoryId,
                CategoryName = comment.Post.Category.Name
            });
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            int postId = comment.PostId;
            db.Comments.Remove(comment);
            db.SaveChanges();

            Flash.Instance.Success("Comment Deleted", "The comment has been deleted");
            return RedirectToAction("details", "posts", new { id = postId });
        }

        public ActionResult Approve(int? id)
        {
            ActionResult result = UpdateCommentApproval(id, approved: true);

            Flash.Instance.Success("Comment Approved", "The comment has been approved");

            return result;
        }

        public ActionResult Disallow(int id)
        {
            ActionResult result = UpdateCommentApproval(id, approved: false);

            Flash.Instance.Success("Comment Disallowed", "The comment has been disallowed");

            return result;
        }

        private ActionResult UpdateCommentApproval(int? id, bool approved)
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

            if (comment.IsApproved != approved)
            {
                comment.IsApproved = approved;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("details", "posts", new { id = comment.PostId });
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
