using FileStorageService.Application.Services;
using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Repos;
using FileStorageService.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using SharedContacts.Clients;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorageSettings"));

// Регистрация репозиториев и сервисов
builder.Services.AddScoped<IFileStoringRepository, FileStoringRepository>();
builder.Services.AddScoped<IFileStoringService, FileStoringService>();

// Регистрация HttpClient для FileAnalysisClient
builder.Services.AddHttpClient<FileAnalysisClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:FileAnalysisService"]!);
});

// Настройка БД
builder.Services.AddDbContext<FileStoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FileStorageDatabase")));

// Minimal API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Конвейер обработки запросов
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

// Применение миграций
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FileStoringDbContext>();
    dbContext.Database.Migrate();
    
    // Создание папки для загрузок
    var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
    Directory.CreateDirectory(uploadsPath);
}

app.Run();

public class FileStorageSettings
{
    public string UploadPath { get; set; } = "uploads";
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024;
}