using WEB.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors.Infrastructure;
using WEB.Api.Services;
using WEB.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// �������� ������ ����������� �� ����� ������������
string connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

var authServer = builder.Configuration.GetSection("AuthServer").Get<AuthServerData>();

// �������� ������ ��������������
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o => {
    // ����� ���������� ������������ OpenID
    o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known / openid - configuration";
// Authority ������� ��������������
o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";
    // Audience ��� ������ JWT
    o.Audience = "account";
    // ��������� HTTPS ��� ������������� ��������� ������ Keycloak
    // � ������� ������� ������ ���� true
    o.RequireHttpsMetadata = false;
});

builder.Services.AddAuthorization(opt => {
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
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

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
