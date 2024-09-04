using Microsoft.AspNetCore.Cors.Infrastructure;
using WEB.Api.Services;
using WEB_253502_Garnik.Services.CategoryService;
using WEB_253502_Garnik.Services.CourceService;
using WEB_253502_Garnik.Services.FileService;

namespace WEB_253502_Garnik.Extensions {
    public static class HostingExtensions {
        public static void RegisterCustomServices(this WebApplicationBuilder builder) {
            //builder.Services.AddScoped<ICategoryService, MemoryCategoryService>();
            //builder.Services.AddScoped<ICourseService, MemoryCourseService>();
            builder.Services.AddHttpClient<ICourseService, ApiCourseService>(opt =>
            opt.BaseAddress = new Uri(UriData.ApiUri));
            builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt =>
            opt.BaseAddress = new Uri(UriData.ApiUri));
            builder.Services.AddHttpClient<IFileService, ApiFileService>(opt =>
            opt.BaseAddress = new Uri($"{UriData.ApiUri}Files"));

        }
    }
}
