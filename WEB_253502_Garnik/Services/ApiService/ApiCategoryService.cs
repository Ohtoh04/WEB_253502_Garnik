using System.Net.Http;
using System.Text;
using System.Text.Json;
using WEB.Domain.Entities;
using WEB.Domain.Models;

namespace WEB.Api.Services
{

    public class ApiCategoryService : ICategoryService {
        private HttpClient _httpClient;
        private JsonSerializerOptions _serializerOptions;
        private ILogger<ApiCourseService> _logger;
        public ApiCategoryService(HttpClient httpClient, IConfiguration configuration,
        ILogger<ApiCourseService> logger)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
        }
        public async Task<ResponseData<List<Category>>> GetCategoryListAsync() {
            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}Categories/");
            // отправить запрос к API
            var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<List<Category>>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Данные не получены от сервера. Error:{response.StatusCode.ToString()}");
            return ResponseData<List<Category>>.Error($"Данные не получены от сервера. Error:{response.StatusCode.ToString()}");
        }
    }
}
