using WEB.Domain.Entities;
using WEB.Domain.Models;

namespace WEB_253502_Garnik.Services.CategoryService {
    public interface ICategoryService {
        /// <summary>
        /// Получение списка всех категорий
        /// </summary>
        /// <returns></returns>
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}
