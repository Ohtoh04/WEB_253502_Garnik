using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using WEB.Domain.Entities;
using WEB_253502_Garnik.Extensions;
using WEB_253502_Garnik.Services.CourceService;

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

        public int page1;
        public int page2;
        public int page3;
        public int currentPage;
        public int totalPages;
        public async Task OnGetAsync(int pageNo = 1) {
            var courseResponse = await _context.GetCourseListAsync(null, pageNo);
            if (courseResponse.Successfull)
                Course.AddRange(courseResponse.Data.Items);


            currentPage = pageNo;
            totalPages = courseResponse.Data.TotalPages;

            page1 = 0; page2 = 0; page3 = 0;
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
           
        }

    }
}
