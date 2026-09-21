using FluentValidation;
using Serilog;

namespace TodoAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _log;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _log = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Произошло необработанное исключение в middleware: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var (statusCode, message) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Доступ запрещен."),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Ресурс не найден."),
                ValidationException => (StatusCodes.Status400BadRequest, exception.Message),

                _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.")
            };
            
            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = statusCode,
                error = message,
            };
            Log.Information("Отправлен ответ клиенту с ошибкой: {Error}, статус: {Status}", response.error, response.status);
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
