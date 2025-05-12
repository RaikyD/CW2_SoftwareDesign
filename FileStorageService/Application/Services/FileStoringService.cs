using FileStorageService.Application.DTO;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;

namespace FileStorageService.Application.Services;

public class FileStoringService : IFileStoringService
{
    private readonly IFileStoringRepository _repository;
    private readonly IWebHostEnvironment _env;

    public FileStoringService(IFileStoringRepository repository, IWebHostEnvironment env)
    {
        _repository = repository;
        _env = env;
    }

    public FileHolder? GetFile(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid file ID.");

        return _repository.Get(id);
    }

    public Guid AddFile(FileUploadDto fileDto, int hash)
    {
        // Проверяем существование файла с таким хешом
        var existingFile = _repository.GetByHash(hash);
        if (existingFile != null)
        {
            return existingFile.Id; // Возвращаем ID существующего файла
        }

        // Создаем путь для сохранения файла
        var uploadsPath = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadsPath);
        var filePath = Path.Combine(uploadsPath, $"{Guid.NewGuid()}.txt");

        // Сохраняем файл на диск
        File.WriteAllText(filePath, fileDto.Content);

        // Создаем новую запись
        var newFile = new FileHolder(
            id: Guid.NewGuid(),
            name: fileDto.FileName,
            hash: hash,
            dir: filePath
        );

        return _repository.Add(newFile);
    }
}