using System.Web.Mvc;

namespace DuckDuckShoot.Controllers
{
    public class GameController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}