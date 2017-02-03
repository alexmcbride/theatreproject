﻿using System.Linq;
using System.Web.Mvc;
using TheatreProject.Models;

namespace TheatreProject.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        [AllowAnonymous]
        public ActionResult Categories(int? id)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Post(int? id)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }
    }
}