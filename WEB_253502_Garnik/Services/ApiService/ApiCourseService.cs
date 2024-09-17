using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using WEB_253502_Garnik.Controllers;
using WEB_253502_Garnik.Services.Authentication;
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
        private ITokenAccessor _tokenAccessor;  // Добавляем ITokenAccessor

        public ApiCourseService(HttpClient httpClient,
                                IConfiguration configuration,
                                ILogger<ApiCourseService> logger,
                                IFileService fileService,
                                ITokenAccessor tokenAccessor) {  // Внедряем ITokenAccessor
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value;
            _serializerOptions = new JsonSerializerOptions() {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
            _fileService = fileService;
            _tokenAccessor = tokenAccessor;  // Сохраняем зависимость
        }

        private async Task SetAuthorizationHeaderAsync() {
            // Вызов метода для установки заголовка авторизации
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
        }

        public async Task<ResponseData<Course>> CreateCourseAsync(Course product, IFormFile? formFile) {
            // Устанавливаем заголовок авторизации
            await SetAuthorizationHeaderAsync();

            if (formFile != null) {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                if (!string.IsNullOrEmpty(imageUrl))
                    product.Image = imageUrl;
            }

            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Courses");
            var response = await _httpClient.PostAsJsonAsync(uri, product, _serializerOptions);
            if (response.IsSuccessStatusCode) {
                var data = await response.Content.ReadFromJsonAsync<ResponseData<Course>>(_serializerOptions);
                return data;
            }
            _logger.LogError($"-----> object not created. Error:{response.StatusCode.ToString()}");
            return ResponseData<Course>.Error($"Объект не добавлен. Error:{response.StatusCode.ToString()}");
        }

        public async Task DeleteCourseAsync(int id) {
            // Устанавливаем заголовок авторизации
            await SetAuthorizationHeaderAsync();

            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Courses" + $"/{id}");
            var delcourse = GetCourseByIdAsync(id).Result.Data ?? new Course();
            await _fileService.DeleteFileAsync(delcourse.Image ?? "");
            var response = await _httpClient.DeleteAsync(uri);
            if (response.IsSuccessStatusCode) {
                return;
            }
            _logger.LogError($"-----> object not deleted. Error:{response.StatusCode.ToString()}");
        }

        public async Task<ResponseData<Course>> GetCourseByIdAsync(int id) {
            // Устанавливаем заголовок авторизации
            await SetAuthorizationHeaderAsync();

            var urlString = $"{_httpClient.BaseAddress.AbsoluteUri}Courses/{id}";
            var response = await _httpClient.GetAsync(new Uri(urlString));

            if (response.IsSuccessStatusCode) {
                try {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Course>>(_serializerOptions);
                }
                catch (JsonException ex) {
                    _logger.LogError($"-----> Error: {ex.Message}");
                    return ResponseData<Course>.Error($"Error: {ex.Message}");
                }
            }

            _logger.LogError($"-----> Data not retrieved from server. Error: {response.StatusCode}");
            return ResponseData<Course>.Error($"Data not retrieved from server. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<ListModel<Course>>> GetCourseListAsync(string? categoryNormalizedName, int pageNo = 1) {
            // Устанавливаем заголовок авторизации
            await SetAuthorizationHeaderAsync();

            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}Courses");
            if (categoryNormalizedName != null) {
                urlString.Append($"/{categoryNormalizedName}");
            }
            if (pageNo > 1) {
                urlString.Append($"?pageNo={pageNo}");
            }
            if (!_pageSize.Equals("3")) {
                urlString.Append(QueryString.Create("pageSize", _pageSize));
            }

            var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));
            if (response.IsSuccessStatusCode) {
                try {
                    var resp = await response.Content.ReadAsStringAsync();
					return await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Course>>>(_serializerOptions);
                }
                catch (JsonException ex) {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<ListModel<Course>>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Данные не получены от сервера. Error:{response.StatusCode.ToString()}");
            return ResponseData<ListModel<Course>>.Error($"Данные не получены от сервера. Error:{response.StatusCode.ToString()}");
        }

        public async Task UpdateCourseAsync(int id, Course product, IFormFile formFile) {
            // Устанавливаем заголовок авторизации
            await SetAuthorizationHeaderAsync();

            if (formFile != null) {
                var imageUrl = await _fileService.SaveFileAsync(formFile);
                if (!string.IsNullOrEmpty(imageUrl)) {
                    await _fileService.DeleteFileAsync(product.Image);
                    product.Image = imageUrl;
                }
            }

            var uri = new Uri(_httpClient.BaseAddress.AbsoluteUri + "Courses" + $"/{id}");
            var response = await _httpClient.PutAsJsonAsync(uri, product, _serializerOptions);

            if (!response.IsSuccessStatusCode) {
                _logger.LogError($"-----> object not updated. Error:{response.StatusCode.ToString()}");
            }
        }
    }

}
