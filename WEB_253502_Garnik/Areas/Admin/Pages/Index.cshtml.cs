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
    public class IndexModel : PageModel
    {
        private readonly ICourseService _context;

        public IndexModel(ICourseService context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Course = _context.GetCourseListAsync(null).Result.Data.Items;
        }
    }
}
