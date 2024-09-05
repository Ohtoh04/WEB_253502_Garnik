using WEB.Domain.Entities;
using WEB.Domain.Models;

namespace WEB_253502_Garnik.Services.CategoryService {
    public class MemoryCategoryService : ICategoryService {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync() {
            var categories = new List<Category>
            {
                new Category {
                    Id = 1,
                    Name = "Languages",
                    NormalizedName = "Languages" },
                new Category {
                    Id = 2,
                    Name = "Driving",
                    NormalizedName = "Driving" },
                new Category {
                    Id = 3,
                    Name = "Autism stimulation",
                    NormalizedName = "Autism" }
            };
                var result = ResponseData<List<Category>>.Success(categories);
            return Task.FromResult(result);
        }
    }
}
