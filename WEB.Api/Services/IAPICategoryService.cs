using WEB.Domain.Entities;
using WEB.Domain.Models;

namespace WEB.Api.Services
{
    public interface IAPICategoryService
    {
        /// <summary>
        /// Получение списка всех категорий
        /// </summary>
        /// <returns></returns>
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}
