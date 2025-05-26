using System.Text;
using FileStorageService.Application.DTO;
using FileStorageService.Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using SharedContacts.Clients;

namespace FileStorageService.Presentation.Controllers;

using Microsoft.AspNetCore.Mvc;

public static class FileStorageApiExtensions
{
    public static RouteGroupBuilder MapFileStorageApi(this RouteGroupBuilder group)
    {
        group.MapPost("files", HandleFileUpload)
            .DisableAntiforgery()                     
            .Accepts<IFormFile>("multipart/form-data")     
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        
        group.MapGet("files/storage/{id}", HandleFileDownload)
            .Produces<FileContentHttpResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapGet("files/storage-request/{id}", HandleFileRequest)
            .Produces(StatusCodes.Status200OK, null,"text/plain; charset=utf-8")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        
        return group;
    }

    public static async Task<IResult> HandleFileUpload(
        IFormFile file,
        [FromServices] IFileStoringService service,
        [FromServices] IFileAnalysisClient analysisClient)
    {
        if (file.Length == 0)
            return Results.BadRequest("File is required");

        using var streamReader = new StreamReader(file.OpenReadStream());
        var content = await streamReader.ReadToEndAsync();

        var analysisResult = await analysisClient.AnalyzeHashAsync(content);

        if (service.HashExists(analysisResult))
            return Results.Ok(service.GetFileIdByHash(analysisResult));

        var newFileId = service.AddFile(
            new FileUploadDto { FileName = file.FileName, Content = content },
            analysisResult
        );
        
        return Results.Created($"/files/{newFileId}", newFileId);
    }

    public static async Task<IResult> HandleFileDownload(
        [FromRoute] Guid id,
        [FromServices] IFileStoringService service)
    {
        var fileHolder = service.GetFile(id);
        if (fileHolder == null)
            return Results.NotFound("File not found");
            
        var content = await service.GetFileContentAsync(fileHolder);
        return Results.File(
            fileContents: content,
            contentType: "application/octet-stream",
            fileDownloadName: fileHolder.Id.ToString()
        );
    }

    public static async Task<IResult> HandleFileRequest(
        [FromRoute] Guid id,
        [FromServices] IFileStoringService service)
    {
        var fileHolder = service.GetFile(id);
        if (fileHolder == null)
            return Results.NotFound("File not found");
            
        var content = await service.GetFileContentAsync(fileHolder);
        var res = Encoding.UTF8.GetString(content);
        return Results.Text(res, "text/plain; charset=utf-8");
    }
}