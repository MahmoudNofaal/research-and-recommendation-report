using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
