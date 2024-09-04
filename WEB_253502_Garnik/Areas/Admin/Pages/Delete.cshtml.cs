using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB.Api.Data;
using WEB.Domain.Entities;

namespace WEB_253502_Garnik.Areas.Admin
{
    public class DeleteModel : PageModel
    {
        private readonly ICourseService _context;

        public DeleteModel(ICourseService context)
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

            var course = await _context.GetCourseByIdAsync(id ?? default(int)); 

            if (course.Data == null)
            {
                return NotFound();
            }
            else
            {
                Course = course.Data; 
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.GetCourseByIdAsync(id ?? default(int));
            if (course.Data != null)
            {
                Course = course.Data;
                await _context.DeleteCourseAsync(id ?? default(int));
            }

            return RedirectToPage("./Index");
        }
    }
}
