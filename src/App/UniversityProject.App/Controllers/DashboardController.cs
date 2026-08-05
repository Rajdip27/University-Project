using Microsoft.AspNetCore.Mvc;

namespace UniversityProject.App.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
