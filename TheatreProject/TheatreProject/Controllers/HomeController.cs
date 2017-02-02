using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheatreProject.Models;

namespace TheatreProject.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var categories = new Category[]
            {
                //new Category { CategoryId=1, Name="Announcements" },
                //new Category { CategoryId=2, Name="News" },
                //new Category { CategoryId=3, Name="Movie Reviews" },
                //new Category { CategoryId=4, Name="Theatre Reviews" },
            };

            return View(categories);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }
    }
}