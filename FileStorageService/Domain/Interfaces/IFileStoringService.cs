using FileStorageService.Application.DTO;
using FileStorageService.Domain.Entities;

namespace FileStorageService.Domain.Interfaces;

public interface IFileStoringService
{
    FileHolder? GetFile(Guid id);
    
    bool HashExists(int hash);
    
    Guid GetFileIdByHash(int hash);
    
    Guid AddFile(FileUploadDto fileUpload, int hash);

    public Task<byte[]> GetFileContentAsync(FileHolder fileHolder);
}