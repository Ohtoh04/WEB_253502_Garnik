using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WEB.Api.Data;
using WEB.Domain.Entities;
using WEB.Domain.Models;

namespace WEB.Api.Services
{
    public class ApiProductService : IAPIProductService {
        private readonly int _maxPageSize = 20;
        private HttpClient _httpClient;
        private string _pageSize;
        private JsonSerializerOptions _serializerOptions;
        private ILogger<ApiProductService> _logger;
        public ApiProductService(HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ApiProductService> logger) {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value;
            _serializerOptions = new JsonSerializerOptions() {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
        }
        public async Task<ResponseData<Course>> CreateProductAsync(Course product) {
            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Courses");
            var response = await _httpClient.PostAsJsonAsync(uri, product, _serializerOptions);
            if (response.IsSuccessStatusCode) {
                var data = await response.Content.ReadFromJsonAsync<ResponseData<Course>>(_serializerOptions);
                return data; 
            }
            _logger.LogError($"-----> object not created. Error:{ response.StatusCode.ToString()}");
            return ResponseData<Course>.Error($"Объект не добавлен. Error:{response.StatusCode.ToString()}");
        }
        public Task DeleteProductAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Course>> GetProductByIdAsync(int id) {
            throw new NotImplementedException();
        }

        public async Task<ResponseData<ListModel<Course>>> GetProductListAsync(
        string? categoryNormalizedName, int pageNo = 1, int pageSize = 3) {
            // подготовка URL запроса
            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}dishes/");
            // добавить категорию в маршрут
            if (categoryNormalizedName != null) {
                urlString.Append($"{categoryNormalizedName}/");
            }
            // добавить номер страницы в маршрут
            if (pageNo > 1) {
                urlString.Append($"page{pageNo}");
            }
            // добавить размер страницы в строку запроса
            if (!_pageSize.Equals("3")) {
                urlString.Append(QueryString.Create("pageSize", _pageSize));
            }
            // отправить запрос к API
            var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));
            if (response.IsSuccessStatusCode) {
                try {
                    return await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Course>>>(_serializerOptions);
                } catch (JsonException ex) {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<ListModel<Course>>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Данные не получены от сервера. Error:{response.StatusCode.ToString()}");
            return ResponseData<ListModel<Course>>.Error($"Данные не получены от сервера. Error:{response.StatusCode.ToString()}");
        }


        public Task UpdateProductAsync(int id, Course product) {
            throw new NotImplementedException();
        }
        public Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile) {
            throw new NotImplementedException();
        }
    }
}
