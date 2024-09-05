using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using WEB.Domain.Entities;

namespace WEB_253502_Garnik.Areas.Admin
{
    public class IndexModel : PageModel
    {
        private readonly ICourseService _context;

        public IndexModel(ICourseService context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; } = new List<Course>();

        public async Task OnGetAsync()
        {
            var coursesResponse = await _context.GetCourseListAsync(null);
            for (int i = 1; i <= coursesResponse.Data.TotalPages; i++) {
                coursesResponse = await _context.GetCourseListAsync(null, i);
                Course.AddRange(coursesResponse.Data.Items);
            }
        }
    }
}
