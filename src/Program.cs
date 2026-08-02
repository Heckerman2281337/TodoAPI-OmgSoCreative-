using Microsoft.AspNetCore.OpenApi;
using TodoAPI.Repo;
using Microsoft.EntityFrameworkCore.Design;
using TodoAPI.Services;
using Microsoft.OpenApi.Models;
using TodoAPI.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

public class Program
{
    public static void Main(string[] args)
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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithAuth();
            builder.Services.AddAuth(builder.Configuration);
            builder.Services.AddDataAcces(builder.Configuration);
            builder.Services.AddBuisnessLogic();
            builder.Services.AddControllers();


            var app = builder.Build();

            /*if (app.Environment.IsDevelopment())
             {
                 app.MapOpenApi();
             }
            */

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
                db.Database.Migrate();
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
