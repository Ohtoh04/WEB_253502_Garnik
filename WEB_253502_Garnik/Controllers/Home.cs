using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_253502_Garnik.Models;

namespace WEB_253502_Garnik.Controllers {
    public class Home : Controller {
        public IActionResult Index(int selectedItem) {
            ViewData["Message"] = "Лабораторная работа №2";
            // Здесь можно обрабатывать выбранный элемент (selectedItem - это Id)
            ViewBag.SelectedId = selectedItem;

            // Повторно создаем список для повторного отображения формы
            var items = new List<ListDemo>
            {
                new ListDemo { Id = 1, Name = "Элемент 1" },
                new ListDemo { Id = 2, Name = "Элемент 2" },
                new ListDemo { Id = 3, Name = "Элемент 3" }
            };
            var Items = new SelectList(items, "Id", "Name");
            return View(Items);
        }
        public IActionResult Lab1() {
            List<string> items = new List<string> { "Элемент1", "Элемент2", "Элемент3", "Элемент4" };
            return View("~/Views/Lab1/Lab1.cshtml", items);
        }
        public IActionResult MenuPartial() {
            return PartialView("~/Views/Lab2/_MenuPartial.cshtml");
        }
        public IActionResult UserInfoPartial() {
            return PartialView("~/Views/Lab2/_UserInfoPartial.cshtml");
        }

    }
}
