using FileStorageService.Application.Services;
using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Repos;
using FileStorageService.Infrastructure.Repositories;
using FileStorageService.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorageSettings"));

// Регистрация сервисов
builder.Services.AddScoped<IFileStoringRepository, FileStoringRepository>();
builder.Services.AddScoped<IFileStoringService, FileStoringService>();

// Регистрация HttpClient для вызова FileAnalysisService
builder.Services.AddHttpClient("AnalysisService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AnalysisService"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Регистрация DbContext
builder.Services.AddDbContext<FileStoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FileStorageDatabase")));

// Настройка Minimal API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Конфигурация HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Регистрация эндпоинтов
app.MapGroup("/api/files")
   .MapFileStorageApi()
   .WithTags("File Storage API");

// Миграции базы данных
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileStoringDbContext>();
    dbContext.Database.Migrate();
    
    // Создаем папку для загрузок, если не существует
    var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
    Directory.CreateDirectory(uploadsPath);
}

app.Run();

// Настройки для хранения файлов (добавьте в appsettings.json)
public class FileStorageSettings
{
    public string UploadPath { get; set; } = "uploads";
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
}