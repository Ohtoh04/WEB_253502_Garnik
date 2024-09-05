using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WEB_253502_Garnik.Services.FileService {
    public class ApiFileService : IFileService {
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;
        private readonly HttpContextAccessor httpContextAccessor;
        public ApiFileService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
        }
        public async Task DeleteFileAsync(string fileUri) {
            // Create the DELETE request message
            var request = new HttpRequestMessage {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(fileUri)
            };

            // Send the DELETE request to the API
            var response = await _httpClient.SendAsync(request);

            // Handle the response, throwing an exception if the request was not successful
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Failed to delete the file. Status code: {response.StatusCode}");
            }
        }


        public async Task<string> SaveFileAsync(IFormFile formFile) {
            // Создать объект запроса
            var request = new HttpRequestMessage {
                Method = HttpMethod.Post
            };
            // Сформировать случайное имя файла, сохранив расширение
            var extension = Path.GetExtension(formFile.FileName);
            var newName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
            // Создать контент, содержащий поток загруженного файла
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(formFile.OpenReadStream());
            content.Add(streamContent, "file", newName);
            // Поместить контент в запрос
            request.Content = content;
            // Отправить запрос к API
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                // Вернуть полученный Url сохраненного файла
                return await response.Content.ReadAsStringAsync();
            }
            return String.Empty;
        }
    }
}
