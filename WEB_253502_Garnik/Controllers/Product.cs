using Microsoft.AspNetCore.Mvc;
using WEB.Api.Services;
using WEB.Domain.Entities;
using WEB_253502_Garnik.Services.CategoryService;
using WEB_253502_Garnik.Services.CourceService;

namespace WEB_253502_Garnik.Controllers {
    public class Product : Controller {
        //ICourseService _courseService;
        //ICategoryService _categoryService;
        ICourseService _courseService;
        ICategoryService _categoryService;
        public Product(ICourseService courseservice, ICategoryService categoryservice) {
            _courseService = courseservice;
            _categoryService = categoryservice;
        }
        public async Task<IActionResult> Catalog(string? category, int pageNo = 1) {
            var courseResponse = await _courseService.GetCourseListAsync(category, pageNo);
            if (!courseResponse.Successfull)
                return NotFound(courseResponse.ErrorMessage);
            ViewBag.Categories = _categoryService.GetCategoryListAsync().Result.Data;
            var cat = _categoryService.GetCategoryListAsync().Result.Data.FirstOrDefault(cat => cat.NormalizedName == category);
            ViewBag.CurrentCategory = cat ?? new Category(); // Replace `Category` with your actual category class.
            return View("~/Views/Lab3/Catalog.cshtml", courseResponse.Data);
        }
    }
}
