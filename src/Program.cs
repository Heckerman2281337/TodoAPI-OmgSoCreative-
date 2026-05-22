using Microsoft.AspNetCore.OpenApi;
using TodoAPI.src.Repo;
using Microsoft.EntityFrameworkCore.Design;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services
            .AddOpenApi()
            .AddDataAcces()
            .AddControllers();

        var app = builder.Build();

        builder.Services.AddDataAcces();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}
