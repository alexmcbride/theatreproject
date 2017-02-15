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
                    ViewBag.Message = "You have signed in.";
                    ViewBag.MessageType = "success";
                    break;
                case AccountMessageId.SignedOut:
                    ViewBag.Message = "You have signed out.";
                    ViewBag.MessageType = "success";
                    break;
            }
        }
    }
}