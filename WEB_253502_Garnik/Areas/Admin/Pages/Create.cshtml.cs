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


        private readonly ICourseService _context;

        public CreateModel(ICourseService context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile? formFile)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.CreateCourseAsync(Course, formFile);

            return RedirectToPage("./Index");
        }
    }
}
