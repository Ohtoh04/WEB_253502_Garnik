using Microsoft.EntityFrameworkCore;
using System.Data;
using WEB.Domain.Entities;

namespace WEB.Api.Data {
    public class DbInitializer  {
        public static async Task SeedData(WebApplication app) {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var apiUrl = app.Configuration["ApplicationSettings:ApiUrl"];

            if (!context.Categories.Any()) {
                var categories = new List<Category>
                {
                    new Category { Name = "Languages", NormalizedName = "Languages" },
                    new Category { Name = "Driving", NormalizedName = "Driving" },
                    new Category { Name = "Autism stimulation", NormalizedName = "Autism" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();

                var courses = new List<Course>
                {
                    new Course {Name="English language",
                                Description="learn inglish sooka",
                                Price = 200, Image=$"{apiUrl}/Images/czipsy.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Languages"))},
                    new Course { Name="German language", Description="deutsch sprache ja ja",
                                Price = 330, Image=$"{apiUrl}/Images/niemecko.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Languages"))},
                    new Course { Name="Tancy s gorinom", Description="bebebebebbebe",
                                Price = 69, Image=$"{apiUrl}/Images/ded.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                    new Course { Name="gddfdjgj", Description="fjghsdshshs",
                                Price = 420, Image=$"{apiUrl}/Images/happy.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                    new Course {Name="bvmcbmcvmvb", Description="asfhfdhfd",
                                Price = 1234, Image=$"{apiUrl}/Images/IMG_20191029_000252.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                    new Course {Name="aboba", Description="asfafafaf",
                                Price = 228, Image=$"{apiUrl}/Images/ladia.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                    new Course { Name="babababba", Description="brebrbwqe",
                                Price = 282, Image=$"{apiUrl}/Images/screen-2.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                    new Course { Name="bebebebbe", Description="ghedfgdgfd",
                                Price = 25, Image=$"{apiUrl}/Images/vozmak.jpg",
                                Category= categories.Find(c=>c.NormalizedName.Equals("Autism"))},
                };

                context.Courses.AddRange(courses);

                await context.SaveChangesAsync();
            }
        }
    }
}
