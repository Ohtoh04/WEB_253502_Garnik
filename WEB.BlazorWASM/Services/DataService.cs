using System.Net.Http.Json;
using System;
using System.Text;
using WEB.Domain.Entities;
using WEB.Domain.Models;
using System.Data.SqlTypes;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace WEB.BlazorWASM.Services {
    public class DataService : IDataService {
		private HttpClient _httpClient;
		private JsonSerializerOptions _serializerOptions;
		private ILogger<DataService> _logger;
		private string _pageSize;
		public DataService(HttpClient httpClient, IConfiguration configuration, ILogger<DataService> logger) {
			_httpClient = httpClient;
			_serializerOptions = new JsonSerializerOptions() {
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
			_logger = logger;
			_pageSize = configuration.GetSection("ItemsPerPage").Value;

			builder.Services.AddHttpClient(opt => opt.BaseAddress = builder.Configuration.GetSection("UriData:ApiUri").Value);
		}
        public List<Category> Categories { get; set; }
        public List<Course> Courses { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Category SelectedCategory { get; set; }

        public event Action DataLoaded;

        public Task GetCategoryListAsync() {
            throw new NotImplementedException();
        }

        public async Task GetProductListAsync(int pageNo = 1) {
			this.CurrentPage = pageNo;

			var route = new StringBuilder("courses/");
			// добавить категорию в маршрут
			if (SelectedCategory is not null) {
				route.Append($"{SelectedCategory.NormalizedName}/");
			};
			List<KeyValuePair<string, string>> queryData = new();
			// добавить номер страницы в маршрут
			if (pageNo > 1) {
				queryData.Add(KeyValuePair.Create("pageNo", pageNo.ToString()));
			};
			// добавить размер страницы
			if (!_pageSize.Equals("3")) {
				queryData.Add(KeyValuePair.Create("pageSize", _pageSize));
			}
			// добавить строку запроса в Url
			if (queryData.Count > 0) {
				route.Append(QueryString.Create(queryData));
			}

			var response = await _httpClient.GetAsync(new Uri(route));

			if (response.IsSuccessStatusCode) {
				try {
					var responseData = await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Course>>>(_serializerOptions);
					Courses = responseData.Result.Data.Items;

					TotalPages = responseData.Result.TotalPages;
					CurrentPage = responseData.Result.CurrentPage;

					// Setting to success
					this.Success = true;
					this.ErrorMessage = String.Empty;
					DataLoaded.Invoke();
				}
				catch (JsonException ex) {
					_logger.LogError($"-----> Ошибка: {ex.Message}");
					this.Success = false;
					this.ErrorMessage =($"Ошибка: {ex.Message}";
				}
			}
			_logger.LogError($"-----> Данные не получены от сервера. Error:{response.StatusCode.ToString()}");
			this.Success = false;
			this.ErrorMessage = $"Данные не получены от сервера. Error:{response.StatusCode.ToString()}";

		}
    }
}
