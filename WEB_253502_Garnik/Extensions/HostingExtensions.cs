using Microsoft.AspNetCore.Cors.Infrastructure;
using WEB_253502_Garnik.Services.CategoryService;
using WEB_253502_Garnik.Services.CourceService;

namespace WEB_253502_Garnik.Extensions {
    public static class HostingExtensions {
        public static void RegisterCustomServices(this WebApplicationBuilder builder) {
            builder.Services.AddScoped<ICategoryService, MemoryCategoryService>();
            builder.Services.AddScoped<ICourseService, MemoryCourseService>();

        }
    }
}
