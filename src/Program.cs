using TodoAPI.Repo;
using TodoAPI.Services;
using TodoAPI.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

public class Program
{
    public static async void Main(string[] args)
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

            var builder = WebApplication.CreateBuilder();
            builder.Host.UseSerilog();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithAuth();
            builder.Services.AddAuth(builder.Configuration);
            builder.Services.AddDataAcces(builder.Configuration);
            builder.Services.AddBuisnessLogic();

            var app = builder.Build();

            /*if (app.Environment.IsDevelopment())
             {
                 app.MapOpenApi();
             }
            */

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                await db.Database.MigrateAsync();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.Message, ex.StackTrace, "Приложение упало с ошибкой {Ex}");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
