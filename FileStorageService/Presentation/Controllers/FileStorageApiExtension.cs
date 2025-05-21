using FileStorageService.Application.DTO;
using FileStorageService.Domain.Interfaces;
using SharedContacts.Clients;
using SharedContacts.DTOs;

namespace FileStorageService.Presentation.Controllers;

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
            [FromServices] FileAnalysisClient analysisClient) =>
        {
            try
            {
                // Анализ содержимого через FileAnalysisClient
                var analysisResult = await analysisClient.AnalyzeContentAsync(request.Content);
                
                // Сохранение файла
                var fileId = service.AddFile(
                    new FileUploadDto
                    {
                        FileName = request.FileName,
                        Content = request.Content
                    },
                    analysisResult.Hash
                );
                
                // Проверка, новый ли файл
                var file = service.GetFile(fileId);
                var isNew = file != null && file.FileName == request.FileName;
                
                return Results.Created($"/api/files/{fileId}", new FileUploadResponse 
                {
                    FileId = fileId,
                    FileName = request.FileName,
                    IsNew = isNew
                });
            }
            catch (HttpRequestException ex)
            {
                return Results.Problem($"Ошибка анализа: {ex.Message}");
            }
        }).Produces<FileUploadResponse>(201).ProducesProblem(500);

        return group;
    }
}