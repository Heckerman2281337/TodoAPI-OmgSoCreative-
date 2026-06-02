using Microsoft.AspNetCore.OpenApi;
using TodoAPI.src.Repo;
using Microsoft.EntityFrameworkCore.Design;
using TodoAPI.src.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDataAcces();
        builder.Services.AddBuisnessLogic();
        builder.Services.AddControllers();

        var app = builder.Build();

       /*if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
       */
        app.UseHttpsRedirection();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        app.Run();
    }
}
