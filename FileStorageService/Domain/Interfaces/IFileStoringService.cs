using FileStorageService.Application.DTO;
using FileStorageService.Domain.Entities;

namespace FileStorageService.Domain.Interfaces;

public interface IFileStoringService
{
    FileHolder? GetFile(Guid id);
    Guid AddFile(FileUploadDto fileUpload, int hash);
}