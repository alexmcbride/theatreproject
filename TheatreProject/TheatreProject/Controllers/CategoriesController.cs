﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TheatreProject.Models;

namespace TheatreProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index(CategoryMessageId? message)
        {
            UpdateMessage(message);

            return View(db.Categories.ToList());
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryId,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("index", "categories", new { message = CategoryMessageId.Added });
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryId,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("index", "categories", new { message = CategoryMessageId.Edited });
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories
                .Include(c => c.Posts)
                .Single(c => c.CategoryId == id);

            // Remove all posts from this category.
            db.Posts.RemoveRange(category.Posts.ToList());

            db.Categories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("index", "categories", new { message = CategoryMessageId.Deleted });
        }

        public enum CategoryMessageId
        {
            None,
            Added,
            Edited,
            Deleted
        }

        private void UpdateMessage(CategoryMessageId? message)
        {
            switch (message ?? CategoryMessageId.None)
            {
                case CategoryMessageId.Added:
                    ViewBag.Message = "The category has been added";
                    ViewBag.MessageType = "success";
                    break;
                case CategoryMessageId.Edited:
                    ViewBag.Message = "The category has been edited";
                    ViewBag.MessageType = "success";
                    break;
                case CategoryMessageId.Deleted:
                    ViewBag.Message = "The category has been deleted";
                    ViewBag.MessageType = "success";
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
