using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Security.Policy;
using WEB_253502_Garnik.Services.Authentication;

namespace WEB_253502_Garnik.Services.FileService {
    public class ApiFileService : IFileService {
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;
        private readonly ITokenAccessor _tokenAccessor;  // Добавляем ITokenAccessor

        public ApiFileService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ITokenAccessor tokenAccessor) {
            _httpClient = httpClient;
            _httpContext = httpContextAccessor.HttpContext;
            _tokenAccessor = tokenAccessor;  // Сохраняем зависимость
        }

        private async Task SetAuthorizationHeaderAsync() {
            // Вызов метода для установки заголовка авторизации
            await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
        }

        public async Task DeleteFileAsync(string fileUri) {
            if (fileUri == null) return;
            // Устанавливаем заголовок авторизации
            await SetAuthorizationHeaderAsync();

            Uri uri = new Uri(fileUri);
            string fileName = Path.GetFileName(uri.LocalPath);
            uri = new Uri($"{_httpClient.BaseAddress.AbsoluteUri}/" + fileName);

            var response = await _httpClient.DeleteAsync(uri);  // Используем _httpClient, а не создаем новый

            // Обработка ответа
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Failed to delete the file. Status code: {response.StatusCode}");
            }
        }

        public async Task<string> SaveFileAsync(IFormFile formFile) {
            // Устанавливаем заголовок авторизации
            await SetAuthorizationHeaderAsync();

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
                // Вернуть полученный URL сохраненного файла
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }
    }

}
