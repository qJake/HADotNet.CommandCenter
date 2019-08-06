using Microsoft.AspNetCore.Mvc;

namespace HADotNet.CommandCenter.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Tiles()
        {
            return View();
        }
        public IActionResult Layout()
        {
            return View();
        }
        public IActionResult Settings()
        {
            return View();
        }
    }
}