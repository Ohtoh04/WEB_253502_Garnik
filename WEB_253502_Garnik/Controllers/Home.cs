using Microsoft.AspNetCore.Mvc;

namespace WEB_253502_Garnik.Controllers {
    public class Home : Controller {
        public IActionResult Index() {
            return View();
        }
        public IActionResult Lab1() {
            return View("~/Views/Lab1/Lab1.cshtml");
        }
    }
}
