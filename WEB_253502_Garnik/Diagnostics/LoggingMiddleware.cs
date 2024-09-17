using Serilog;
using System.Diagnostics;

namespace WEB_253502_Garnik.Diagnostics {
    public class LoggingMiddleware {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            var watch = Stopwatch.StartNew();

            // Пропускаем запрос дальше по конвейеру
            await _next(context);

            watch.Stop();

            var statusCode = context.Response.StatusCode;
            if (statusCode < 200 || statusCode >= 300) {
                var requestPath = context.Request.Path;
                Log.Information("----> request {RequestPath} returns {StatusCode}", requestPath, statusCode);
            }
        }
    }
}
