using TodoAPI.Repo;
using TodoAPI.Services;
using TodoAPI.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

public class Program
{
    public static async Task Main(string[] args)
    {

        Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                    .CreateLogger();
        try
        {
            Log.Information("Старт приложения");

            var builder = WebApplication.CreateBuilder(args);
            Log.Information("Builder создан");

            builder.Host.UseSerilog();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithAuth();
            builder.Services.AddAuth(builder.Configuration);
            builder.Services.AddAuthorization();
            builder.Services.AddDataAcces(builder.Configuration);
            builder.Services.AddBuisnessLogic();
            Log.Information("Сервисы зарегистрированы");

            var app = builder.Build();
            Log.Information("App создан");

            using (var scope = app.Services.CreateScope())
            {
                Log.Information("Запуск миграций");

                var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                await db.Database.MigrateAsync();

                Log.Information("Миграции завершены");
            }

            app.UseMiddleware<ExceptionMiddleware>();
            Log.Information("ExceptionMiddleware OK");
            app.UseAuthentication();
            Log.Information("Authentication OK");
            app.UseAuthorization();
            Log.Information("Authorization OK");
            app.UseSwagger();
            Log.Information("Swagger OK");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoAPI v1");
                c.RoutePrefix = "swagger";
            });
            Log.Information("SwaggerUI OK");
            app.MapControllers();
            Log.Information("Controllers mapped");
            Log.Information("Перед RunAsync");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение упало");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
