using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB.Api.Data;
using WEB.Domain.Entities;
using WEB_253502_Garnik.Services.CourceService;

namespace WEB_253502_Garnik.Areas.Admin
{
    public class EditModel : PageModel
    {
        private readonly ICourseService _context;

        public EditModel(ICourseService context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course =  await _context.GetCourseByIdAsync(id ?? default(int));
            if (course.Data == null)
            {
                return NotFound();
            }
            Course = course.Data;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            try {
                // Assuming Course.ID is being set correctly in the model
                await _context.UpdateCourseAsync(Course.ID, Course, null);
            } catch (DbUpdateConcurrencyException) {
                var courseExists = await _context.GetCourseByIdAsync(Course.ID);
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
            return _context.GetCourseListAsync(null).Result.Data.Items.Any(e => e.ID == id);
        }
    }
}
