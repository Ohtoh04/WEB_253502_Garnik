using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using WEB_253502_Garnik.Controllers;
using WEB_253502_Garnik.Services.FileService;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            product.Image = "/Images/noimage.jpg";
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


        //will do
        public async Task DeleteCourseAsync(int id) {
            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Courses" + $"/{id}");

            var delcourse = GetCourseByIdAsync(id).Result.Data ?? new Course();
            await _fileService.DeleteFileAsync(delcourse.Image ?? "");
            var response = await _httpClient.DeleteAsync(uri);
            if (response.IsSuccessStatusCode) {
                return;
            }
            _logger.LogError($"-----> object not Deleetd. Error:{response.StatusCode.ToString()}");
        }

        public async Task<ResponseData<Course>> GetCourseByIdAsync(int id) {
            // Prepare the URL string for the API request
            var urlString = $"{_httpClient.BaseAddress.AbsoluteUri}Courses/{id}";

            // Send the request to the API
            var response = await _httpClient.GetAsync(new Uri(urlString));

            if (response.IsSuccessStatusCode) {
                try {
                    // Deserialize the response content into a ResponseData<Course> object
                    return await response.Content.ReadFromJsonAsync<ResponseData<Course>>(_serializerOptions);
                } catch (JsonException ex) {
                    // Log the error and return an error response
                    _logger.LogError($"-----> Error: {ex.Message}");
                    return ResponseData<Course>.Error($"Error: {ex.Message}");
                }
            }

            // Log if the data was not successfully retrieved and return an error response
            _logger.LogError($"-----> Data not retrieved from server. Error: {response.StatusCode}");
            return ResponseData<Course>.Error($"Data not retrieved from server. Error: {response.StatusCode}");
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

        //fin?
        public async Task UpdateCourseAsync(int id, Course product, IFormFile formFile) {
            product.Image = "/Images/noimage.jpg";
            // Сохранить файл изображения
            if (formFile != null) {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                // Добавить в объект Url изображения
                if (!string.IsNullOrEmpty(imageUrl)) {
                    await _fileService.DeleteFileAsync(product.Image);
                    product.Image = imageUrl;
                }

            }

            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Courses" + $"/{id}");

            var response = await _httpClient.PutAsJsonAsync(uri, product, _serializerOptions);

            if (!response.IsSuccessStatusCode) {

                _logger.LogError($"-----> object not created. Error:{response.StatusCode.ToString()}");
            }

        }
    }
}
