using System.Net.Http.Json;
using System;
using System.Text;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using System.Data.SqlTypes;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;

namespace WEB.BlazorWASM.Services {
    public class DataService : IDataService {
        private HttpClient _httpClient;
        private JsonSerializerOptions _serializerOptions;
        private ILogger<DataService> _logger;
        private string _pageSize;
        private IAccessTokenProvider _tokenProvider;

        public DataService(HttpClient httpClient, IConfiguration configuration, ILogger<DataService> logger, IAccessTokenProvider accessTokenProvider) {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions() {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
            _pageSize = configuration.GetSection("ItemsPerPage").Value;
            _tokenProvider = accessTokenProvider;
        }

        public List<Category> Categories { get; set; }
        public List<Course> Courses { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Category SelectedCategory { get; set; }

        public event Action DataLoaded;

        private async Task AddAuthorizationHeaderAsync() {
            var result = await _tokenProvider.RequestAccessToken();
            if (result.TryGetToken(out var token)) {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.Value);
            }
            else {
                _logger.LogError("Не удалось получить токен доступа.");
                throw new InvalidOperationException("Токен доступа отсутствует.");
            }
        }

        public async Task GetCategoryListAsync() {
            try {
                await AddAuthorizationHeaderAsync();
            }
            catch (Exception ex) {
                this.Success = false;
                this.ErrorMessage = ex.Message;
                _logger.LogError($"Ошибка авторизации: {ex.Message}");
                return;
            }

            var route = "Categories/";

            HttpResponseMessage? response = await _httpClient.GetAsync(route);

            if (response.IsSuccessStatusCode) {
                try {
                    var responseData = await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);
                    if (responseData != null) {
                        Categories = responseData.Data ?? new List<Category>();
                    }
                    this.Success = true;
                    this.ErrorMessage = string.Empty;
                    DataLoaded.Invoke();
                }
                catch (JsonException ex) {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    this.Success = false;
                    this.ErrorMessage = $"Ошибка: {ex.Message}";
                }
            }
            else {
                _logger.LogError($"-----> Данные не получены от сервера. Error: {response.StatusCode}");
                this.Success = false;
                this.ErrorMessage = $"Данные не получены от сервера. Error: {response.StatusCode}";
            }
        }

        public async Task GetProductListAsync(int pageNo = 1) {
            this.CurrentPage = pageNo;

            try {
                await AddAuthorizationHeaderAsync();
            }
            catch (Exception ex) {
                this.Success = false;
                this.ErrorMessage = ex.Message;
                _logger.LogError($"Ошибка авторизации: {ex.Message}");
                return;
            }

            var route = new StringBuilder("courses");
            if (SelectedCategory is not null) {
                route.Append($"{SelectedCategory.NormalizedName}/");
            }

            List<KeyValuePair<string, string>> queryData = new();
            if (pageNo > 1) {
                queryData.Add(KeyValuePair.Create("pageNo", pageNo.ToString()));
            }
            if (!_pageSize.Equals("3")) {
                queryData.Add(KeyValuePair.Create("pageSize", _pageSize));
            }

            if (queryData.Count > 0) {
                route.Append(QueryString.Create(queryData));
            }

            HttpResponseMessage? response = null;
            try {
                response = await _httpClient.GetAsync(route.ToString());
            }
            catch (Exception ex) {
                this.Success = false;
                this.ErrorMessage = ($"Ошибка: {ex.Message}");
                return;
            }

            if (response.IsSuccessStatusCode) {
                try {
                    var responseData = await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Course>>>(_serializerOptions);
                    if (responseData != null) {
                        Courses = responseData.Data.Items;
                        TotalPages = responseData.Data.TotalPages;
                        CurrentPage = responseData.Data.CurrentPage;
                        this.Success = true;
                        this.ErrorMessage = string.Empty;
                        DataLoaded.Invoke();
                    }
                }
                catch (JsonException ex) {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    this.Success = false;
                    this.ErrorMessage = $"Ошибка: {ex.Message}";
                }
            }
            else {
                _logger.LogError($"-----> Данные не получены от сервера. Error: {response.StatusCode}");
                this.Success = false;
                this.ErrorMessage = $"Данные не получены от сервера. Error: {response.StatusCode}";
            }
        }
    }

}
