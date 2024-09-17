using WEB_253502_Garnik.Extensions;
using WEB_253502_Garnik;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WEB_253502_Garnik.HelperClasses;
using Serilog;
using WEB_253502_Garnik.Diagnostics;
var builder = WebApplication.CreateBuilder(args);

var keycloakData = builder.Configuration.GetSection("Keycloak").Get<KeycloakData>();
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie().AddJwtBearer().AddOpenIdConnect(options => {
    options.Authority = $"{keycloakData.Host}/auth/realms/{keycloakData.Realm}";
    options.ClientId = keycloakData.ClientId;
    options.ClientSecret = keycloakData.ClientSecret;
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.Scope.Add("openid"); // Customize scopes as needed
    options.SaveTokens = true;
    options.RequireHttpsMetadata = false; // позволяет обращаться к локальному Keycloak по http
    options.MetadataAddress = $"{keycloakData.Host}/realms/{keycloakData.Realm}/.well-known/openid-configuration";
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.RegisterCustomServices();

// Получение конфигурации из appsettings.json
var configuration = builder.Configuration;

// Получение значения из раздела UriData
UriData.ApiUri = configuration.GetSection("UriData:ApiUri").Value;
Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(configuration)  // Читаем конфигурацию Serilog из appsettings.json
           .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();


// Add this to map Razor Pages
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
