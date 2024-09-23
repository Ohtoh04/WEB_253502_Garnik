using WEB.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors.Infrastructure;
using WEB.Api.Services;
using WEB.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// получаем строку подключения из файла конфигурации
string connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

var authServer = builder.Configuration.GetSection("AuthServer").Get<AuthServerData>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowSpecificOrigins",
        policy => policy.WithOrigins("https://localhost:44325", "https://localhost:44344")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());

});

builder.Services.AddAuthorization(opt => {
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
});

// Добавить сервис аутентификации
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o => {
    // Адрес метаданных конфигурации OpenID
    o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known/openid-configuration";
    // Authority сервера аутентификации
    o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";
    // Audience для токена JWT
    o.Audience = "account";
    // Запретить HTTPS для использования локальной версии Keycloak
    // В рабочем проекте должно быть true
    o.RequireHttpsMetadata = false;

    o.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudiences = new[] { "account", "GarnikUiClient", "master-realm", "GarnikWASM" },  // Multiple audiences
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});





builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAPICategoryService, CategoryService>();

builder.Services.AddScoped<IAPIProductService, ProductService>();

var app = builder.Build();

await DbInitializer.SeedData(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();


app.Run();
