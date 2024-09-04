using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using WEB_253502_Garnik.Services.FileService;

namespace WEB.Api.Services
{
    public class ApiCourseService : ICourseService {
        private readonly int _maxPageSize = 20;
        private HttpClient _httpClient;
        private string _pageSize;
        private JsonSerializerOptions _serializerOptions;
        private ILogger<ApiCourseService> _logger;
        private IFileService _fileService;


        public ApiCourseService(HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ApiCourseService> logger, IFileService fileService) {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value;
            _serializerOptions = new JsonSerializerOptions() {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
            _fileService = fileService;
        }


        public async Task<ResponseData<Course>> CreateCourseAsync(Course product, IFormFile? formFile) {
            product.Image = "Images/noimage.jpg";
            // Сохранить файл изображения
            if (formFile != null) {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                // Добавить в объект Url изображения
                if (!string.IsNullOrEmpty(imageUrl))
                    product.Image = imageUrl;
            }

            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Courses");
            var response = await _httpClient.PostAsJsonAsync(uri, product, _serializerOptions);
            if (response.IsSuccessStatusCode) {
                var data = await response.Content.ReadFromJsonAsync<ResponseData<Course>>(_serializerOptions);
                return data; 
            }
            _logger.LogError($"-----> object not created. Error:{ response.StatusCode.ToString()}");
            return ResponseData<Course>.Error($"Объект не добавлен. Error:{response.StatusCode.ToString()}");
        }


        public Task DeleteCourseAsync(int id) {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Course>> GetCourseByIdAsync(int id) {
            throw new NotImplementedException();
        }

        public async Task<ResponseData<ListModel<Course>>> GetCourseListAsync(
        string? categoryNormalizedName, int pageNo = 1) {
            // подготовка URL запроса
            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}Courses");
            // добавить категорию в маршрут
            if (categoryNormalizedName != null) {
                urlString.Append($"/{categoryNormalizedName}");
            }
            // добавить номер страницы в маршрут
            if (pageNo > 1) {
                urlString.Append($"?pageNo={pageNo}");
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


        public Task UpdateCourseAsync(int id, Course product, IFormFile formFile) {
            throw new NotImplementedException();
        }
        public Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile) {
            throw new NotImplementedException();
        }
    }
}
