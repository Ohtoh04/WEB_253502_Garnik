using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB.Domain.Entities;

namespace WEB_253502_Garnik.Areas.Admin
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public IFormFile? Image { get; set; }

        [BindProperty]
        public Course Course { get; set; } = default!;

        [BindProperty]
        public int categoryID { get; set; }

        public Category CurrentCategory { get; set; } = new Category();
        public List<Category> Categories { get; set; } = new List<Category>();

        private readonly ICourseService _context;
        private readonly ICategoryService _categoryService;

        public CreateModel(ICourseService context, ICategoryService categoryContext)
        {
            _context = context;
            _categoryService = categoryContext;
        }

        public IActionResult OnGet()
        {
            Categories = _categoryService.GetCategoryListAsync().Result.Data ?? new List<Category>();
            CurrentCategory = new Category();
            categoryID = CurrentCategory.Id;
            return Page();
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Course.Category = (_categoryService.GetCategoryListAsync().Result.Data ?? new List<Category>())
                .FirstOrDefault(cat => cat.Id == categoryID);
            await _context.CreateCourseAsync(Course, Image);

            return RedirectToPage("./Index");
        }
    }
}
