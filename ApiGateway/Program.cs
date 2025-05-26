using Microsoft.AspNetCore.Mvc;
using SharedContacts.Clients;
using SharedContacts.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Swagger для Gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<FileStorageClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["FileStorageService:Url"]!));

builder.Services.AddHttpClient<FileAnalysisClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["FileAnalysisService:Url"]!));

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
        c.RoutePrefix = ""; // UI на корне
    });
}


// POST /api/files  — загрузить файл
app.MapPost("/api/files", async 
        (   IFormFile file, 
            [FromServices] FileStorageClient storage) =>
{
    if (file.Length == 0)
        return Results.BadRequest("File is required");

    using var ms = new MemoryStream();
    await file.CopyToAsync(ms);
    ms.Position = 0;

    var id = await storage.UploadAsync(ms, file.FileName);
    return Results.Created($"/api/files/{id}", id);
}).DisableAntiforgery()                     
.Accepts<IFormFile>("multipart/form-data")
.Produces<Guid>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.WithName("UploadFile")
.WithTags("Files");

// GET /api/files/{id}  — содержимое файла
app.MapGet("/api/files/{id}", async (
        Guid id,
        [FromServices] FileStorageClient storage) =>
{
    try
    {
        var content = await storage.GetFileContentAsync(id);
        return Results.Ok(content);
    }
    catch (HttpRequestException)
    {
        return Results.NotFound();
    }
})
.Produces<string>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetFileContent")
.WithTags("Files");

// GET /api/files/{id}/analysis — статистика файла
app.MapGet("/api/files/{id}/analysis", async (
        Guid id,
        [FromServices] FileAnalysisClient analysis) =>
{
    var stats = await analysis.GetStatsAsync(id);
    if (stats == null)
        return Results.NotFound();
    return Results.Ok(stats);
})
.Produces<FileAnalysisResult>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetFileAnalysis")
.WithTags("Analysis");

app.Run();