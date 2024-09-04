using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WEB_253502_Garnik.Services.FileService {
    public class ApiFileService : IFileService {
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;
        private readonly HttpContextAccessor httpContextAccessor;
        public ApiFileService(HttpClient httpClient) {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
        }
        public async Task DeleteFileAsync(string fileUri) {//unfin
            // Создать объект запроса
            var request = new HttpRequestMessage {
                Method = HttpMethod.Delete
            };
            request.Content = new StringContent(fileUri);
            var response = await _httpClient.SendAsync(request);

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
