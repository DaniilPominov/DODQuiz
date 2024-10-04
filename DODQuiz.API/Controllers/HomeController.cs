using Microsoft.AspNetCore.Mvc;

namespace DODQuiz.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
