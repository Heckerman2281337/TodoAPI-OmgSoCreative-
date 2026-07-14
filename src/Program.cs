using Microsoft.AspNetCore.OpenApi;
using TodoAPI.Repo;
using Microsoft.EntityFrameworkCore.Design;
using TodoAPI.Services;
using Microsoft.OpenApi.Models;
using TodoAPI.Middlewares;
using Microsoft.EntityFrameworkCore;
public class Program
{
    public static void Main(string[] args)
    {
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
}
