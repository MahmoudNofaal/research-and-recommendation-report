using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PresetsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
