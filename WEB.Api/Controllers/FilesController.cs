using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace WEB.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase {
        // Путь к папке wwwroot/Images
        private readonly string _imagePath;

        public FilesController(IWebHostEnvironment webHost) {
            _imagePath = Path.Combine(webHost.WebRootPath, "Images");
        }

        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile file) {
            if (file is null) {
                return BadRequest();
            }

            // путь к сохраняемому файлу
            var filePath = Path.Combine(_imagePath, file.FileName);
            var fileInfo = new FileInfo(filePath);

            // если такой файл существует, удалить его
            if (fileInfo.Exists) {
                fileInfo.Delete();
            }
            // скопировать файл в поток
            using var fileStream = fileInfo.Create();
            await file.CopyToAsync(fileStream);

            // получить Url файла
            var host = HttpContext.Request.Host;
            var fileUrl = $"Https://{host}/Images/{file.FileName}";
            return Ok(fileUrl);
        }


        [HttpDelete]
        public IActionResult DeleteFile(string fileName) {
            //. . .
            return Ok();
        }
    }
}
