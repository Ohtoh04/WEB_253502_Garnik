using Microsoft.AspNetCore.Cors.Infrastructure;
using WEB.Api.Services;
using WEB_253502_Garnik.HelperClasses;
using WEB_253502_Garnik.Services.Authentication;
using WEB_253502_Garnik.Services.Authorization;
using WEB_253502_Garnik.Services.CartService;
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
            builder.Services.Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"));
            builder.Services.AddHttpClient<ITokenAccessor, KeycloakTokenAccessor>();
            builder.Services.AddHttpClient<IAuthService, KeycloakAuthService>();
            builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
        }
    }
}
