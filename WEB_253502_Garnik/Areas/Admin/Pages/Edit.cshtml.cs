using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB.Domain.Entities;
using WEB_253502_Garnik.Services.CourceService;

namespace WEB_253502_Garnik.Areas.Admin
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public IFormFile? file { get; set; }

        [BindProperty]
        public int categoryID { get; set; }
        [BindProperty]
        public Course Course { get; set; } = null;
        public Category CurrentCategory { get; set; } = new Category();
        public List<Category> Categories { get; set; } = new List<Category>();


        private readonly ICourseService _context;
        private readonly ICategoryService _categoryService;


        public EditModel(ICourseService context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }





        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Categories = _categoryService.GetCategoryListAsync().Result.Data ?? new List<Category>();

            
            var course =  await _context.GetCourseByIdAsync(id ?? default(int));
            if (course.Data == null)
            {
                return NotFound();
            }
            Course = course.Data;
            var cat = Categories.FirstOrDefault(cat => cat.Id == Course.Category?.Id);
            CurrentCategory = cat ?? new Category(); // Replace `Category` with your actual category class.
            categoryID = CurrentCategory.Id;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }
           
            try {

                Course.Category = (_categoryService.GetCategoryListAsync().Result.Data ?? new List<Category>())
                    .FirstOrDefault(cat => cat.Id == categoryID);
                await _context.UpdateCourseAsync(Course.Id, Course, file);
            } catch (DbUpdateConcurrencyException) {
                var courseExists = await _context.GetCourseByIdAsync(Course.Id);
                if (courseExists.Data == null) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }


        private bool CourseExists(int id)
        {
            return _context.GetCourseListAsync(null).Result.Data.Items.Any(e => e.Id == id);
        }
    }
}
