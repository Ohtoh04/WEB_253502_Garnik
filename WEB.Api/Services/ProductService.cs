using Microsoft.EntityFrameworkCore;
using WEB.Api.Data;
using WEB.Domain.Entities;
using WEB.Domain.Models;

namespace WEB.Api.Services
{
    public class ProductService : IAPIProductService {
        private readonly int _maxPageSize = 20;
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context) {
            _context = context;
        }
        public Task<ResponseData<Course>> CreateProductAsync(Course product) {
                throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Course>> GetProductByIdAsync(int id) {
            throw new NotImplementedException();
        }

        public async Task<ResponseData<ListModel<Course>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1, int pageSize = 3)
        {
            if (pageSize > _maxPageSize)
                pageSize = _maxPageSize;

            var query = _context.Courses
                                .Include(c => c.Category) // Eagerly load the Category entity
                                .AsQueryable();

            var dataList = new ListModel<Course>();

            query = query.Where(d => categoryNormalizedName == null ||
                                     d.Category.NormalizedName.Equals(categoryNormalizedName));

            // количество элементов в списке
            var count = await query.CountAsync();
            if (count == 0)
            {
                return ResponseData<ListModel<Course>>.Success(dataList);
            }

            // количество страниц
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageNo > totalPages)
                return ResponseData<ListModel<Course>>.Error("No such page");

            dataList.Items = await query.OrderBy(d => d.ID)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            dataList.CurrentPage = pageNo;
            dataList.TotalPages = totalPages;

            return ResponseData<ListModel<Course>>.Success(dataList);
        }


        public Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile) {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, Course product) {
            throw new NotImplementedException();
        }
    }
}
