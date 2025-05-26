using FileAnalysisService.Application.Services;
using FileAnalysisService.Domain.Interfaces;
using FileAnalysisService.Infrastructure.Repos;
using FileAnalysisService.Presentation.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SharedContacts.Clients;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов авторизации
builder.Services.AddAuthorization(); 

// Регистрация БД
builder.Services.AddDbContext<FileAnalysisDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("FileAnalysisDatabase")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IFileAnalysisRepository, FileAnalysisRepository>();
builder.Services.AddScoped<IFileAnalysisService, FileAnalysisService.Application.Services.FileAnalysisService>();
builder.Services.AddHttpClient<IFileStorageClient, FileStorageClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["FileStorageService:Url"]!));

// Настройка Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileAnalysisService v1"));
}

// Миграции и создание папки
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FileAnalysisDbContext>();
    db.Database.Migrate();
}

// Регистрация эндпоинтов
app.MapGroup("/api")
    .MapFileAnalysisApi()
    .WithTags("File Analysis API");

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();

public partial class Program { }
