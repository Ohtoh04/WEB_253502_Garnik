using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using WEB_253502_Garnik.HelperClasses;
using WEB_253502_Garnik.Services.Authentication;
using WEB_253502_Garnik.Services.FileService;

namespace WEB_253502_Garnik.Services.Authorization {
    public class KeycloakAuthService : IAuthService {
        private readonly HttpClient _httpClient;
        private readonly IFileService _fileService;
        private readonly ITokenAccessor _tokenAccessor;
        KeycloakData _keycloakData;
        public KeycloakAuthService(HttpClient httpClient,
        IOptions<KeycloakData> options,
        IFileService fileService,
        ITokenAccessor tokenAccessor) {
            _httpClient = httpClient;
            _fileService = fileService;
            _tokenAccessor = tokenAccessor;
            _keycloakData = options.Value;
        }
        public async Task<(bool Result, string ErrorMessage)> RegisterUserAsync(
        string email, string password, IFormFile? avatar) {
            // добавить JWT token в заголовки
            try {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient);
            }
            catch (Exception ex) {
                return (false, ex.Message);
            }
            var avatarUrl = "/images/default-profile-picture.png";
            // сохранить Avatar, если аватар был передан при регистрации
            if (avatar != null) {
                var result = await _fileService.SaveFileAsync(avatar);
                if (result != null) avatarUrl = result;
            }
            // Подготовка данных нового пользователя
            var newUser = new CreateUserModel();
            newUser.Attributes.Add("avatar", avatarUrl);
            newUser.Email = email;
            newUser.Username = email;
            newUser.Credentials.Add(new UserCredentials { Value = password });
            // Keycloak user endpoint
            var requestUri = $"{_keycloakData.Host}/admin/realms/{_keycloakData.Realm}/users";
            // Подготовить контент запроса
            var serializerOptions = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var userData = JsonSerializer.Serialize(newUser, serializerOptions);
            HttpContent content = new StringContent(userData, Encoding.UTF8, "application/json");
            // Отправить запрос
            var response = await _httpClient.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode) return (true, String.Empty);
            return (false, response.StatusCode.ToString());
        }
    }
}
