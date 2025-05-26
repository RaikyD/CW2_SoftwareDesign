using FileStorageService.Application.Services;
using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Repos;
using FileStorageService.Presentation.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedContacts.Clients;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов авторизации
builder.Services.AddAuthorization();

// Регистрация БД
builder.Services.AddDbContext<FileStoringDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("FileStorageDatabase")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IFileStoringRepository, FileStoringRepository>();
builder.Services.AddScoped<IFileStoringService, FileStoringService>();
builder.Services.AddHttpClient<IFileAnalysisClient, FileAnalysisClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["FileAnalysisService:Url"]!));

// Настройка Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStoringService v1"));
}

// Миграции и создание папки
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FileStoringDbContext>();
    db.Database.Migrate();
    
    var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
    Directory.CreateDirectory(uploadsPath);
}

// Регистрация эндпоинтов
app.MapGroup("/api")
    .MapFileStorageApi()
    .WithTags("File Storage API");

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();
public partial class Program { }

public class FileStorageSettings
{
    public string UploadPath { get; set; } = "uploads";
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024;
}