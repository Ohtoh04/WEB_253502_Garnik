using WEB.Api.Data;
using WEB.Domain.Entities;
using WEB.Domain.Models;

namespace WEB.Api.Services
{

    public class CategoryService : IAPICategoryService {
        private readonly AppDbContext _context;
        public async Task<ResponseData<List<Category>>> GetCategoryListAsync() {
            var data = _context.Categories;
            var dataList = new List<Category>();
            dataList.AddRange(data);
            return ResponseData<List<Category>>.Success(dataList);
        }
    }
}
