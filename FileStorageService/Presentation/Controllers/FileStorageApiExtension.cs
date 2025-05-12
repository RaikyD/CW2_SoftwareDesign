using FileStorageService.Application.DTO;
using FileStorageService.Domain.Interfaces;
using SharedContacts.DTOs;

namespace FileStorageService.Presentation.Controllers;

using FileStorageService.Application.Services;
using FileStorageService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

public static class FileStorageApiExtensions
{
    public static RouteGroupBuilder MapFileStorageApi(this RouteGroupBuilder group)
    {
        group.MapGet("/api/content/{id:guid}", async (
            [FromRoute] Guid id,
            [FromServices] IFileStoringService service) =>
        {
            var file = service.GetFile(id);
            if (file == null) return Results.NotFound();
            
            var content = await File.ReadAllTextAsync(file.FileDirectory);
            return Results.Ok(content);
        }).Produces<string>(200).Produces(404);

        group.MapPost("/api", async (
            [FromBody] FileUploadRequest request,
            [FromServices] IFileStoringService service,
            [FromServices] HttpClient httpClient) =>
        {
            // Анализ содержимого
            var analysisResponse = await httpClient.PostAsJsonAsync(
                "http://file-analysis-service/api/analyze",
                new { request.Content });
    
            if (!analysisResponse.IsSuccessStatusCode)
                return Results.Problem("Analysis service unavailable");
    
            var analysisResult = await analysisResponse.Content.ReadFromJsonAsync<AnalysisResult>();
    
            // Сохранение файла (сервис сам проверит уникальность)
            var fileId = service.AddFile(
                new FileUploadDto
                {
                    FileName = request.FileName,
                    Content = request.Content
                },
                analysisResult.Hash
            );
    
            // Получаем файл для проверки, новый он или существующий
            var file = service.GetFile(fileId);
            var isNew = file != null && file.FileName == request.FileName;
    
            return Results.Created($"/api/files/{fileId}", new 
            {
                FileId = fileId,
                FileName = request.FileName,
                IsNew = isNew
            });
        }).Produces(201).ProducesProblem(500);

        return group;
    }
}

public record AnalysisResult(int Hash, int WordCount);