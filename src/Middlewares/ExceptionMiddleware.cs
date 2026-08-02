using Serilog;

namespace TodoAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Serilog.ILogger _log = Log.ForContext<ExceptionMiddleware>();

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Произошло необработанное исключение в middleware: {Message}", ex.Message);
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

                _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.")
            };
            
            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = statusCode,
                error = message,
            };
            _log.Information("Отправлен ответ клиенту с ошибкой: {Error}, статус: {Status}", response.error, response.status);
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
