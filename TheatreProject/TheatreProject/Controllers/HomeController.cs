using System.Linq;
using System.Web.Mvc;
using TheatreProject.Models;

namespace TheatreProject.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Home/Index
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: /Home/Contact
        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }
    }
}