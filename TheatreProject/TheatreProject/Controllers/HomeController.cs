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
        public ActionResult Index(AccountMessageId? message)
        {
            UpdateMessage(message);

            return View(db.Categories.ToList());
        }

        // GET: /Home/Contact
        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        private void UpdateMessage(AccountMessageId? message)
        {
            switch(message ?? AccountMessageId.None)
            {
                case AccountMessageId.SignedIn:
                    ViewData["Message"] = "You have signed in.";
                    ViewData["MessageType"] = "success";
                    break;
                case AccountMessageId.SignedOut:
                    ViewData["Message"] = "You have signed out.";
                    ViewData["MessageType"] = "success";
                    break;
            }
        }
    }
}