using Microsoft.AspNetCore.Mvc;
using WEB.Api.Services;
using WEB.Domain.Entities;
using WEB_253502_Garnik.Extensions;
using WEB_253502_Garnik.Models;
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

            ViewBag.IsAjaxRequest = Request.IsAjaxRequest();
            if (Request.IsAjaxRequest()) {
                int currentPage = pageNo;
                int totalPages = courseResponse.Data.TotalPages;

                int page1 = 0, page2 = 0, page3 = 0;
                if (currentPage == 1) {
                    page1 = 1;
                    page2 = totalPages >= 2 ? 2 : 0;
                    page3 = totalPages >= 3 ? 3 : 0;
                }
                else if (currentPage > 1 && currentPage < totalPages) {
                    page1 = currentPage - 1;
                    page2 = currentPage;
                    page3 = currentPage + 1;
                }
                else if (currentPage == totalPages) {
                    page3 = currentPage;
                    page2 = currentPage - 1;
                    page1 = currentPage - 2 > 0 ? currentPage - 2 : 0;
                }

                var viewModel = new ProductPageViewModel {
                    CurrentPage = currentPage,
                    TotalPages = totalPages,
                    Page1 = page1,
                    Page2 = page2,
                    Page3 = page3,
                    CurrentCategory = cat,
                    Courses = courseResponse.Data.Items
                };
                ViewBag.viewModel = viewModel;
                return PartialView("_ListPartial", viewModel);

            }
            return View("~/Views/Product/Catalog.cshtml", courseResponse.Data);
        }
    }
}
