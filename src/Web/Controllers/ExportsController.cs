using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ExportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
