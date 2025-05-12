using FileStorageService.Domain.Entities;

namespace FileStorageService.Domain.Interfaces;

public interface IFileStoringRepository
{
    FileHolder? Get(Guid id);
    FileHolder? GetByHash(int hash);
    IReadOnlyList<FileHolder>? GetAll();
    Guid Add(FileHolder fileHolder);
    bool Delete(Guid id);
}