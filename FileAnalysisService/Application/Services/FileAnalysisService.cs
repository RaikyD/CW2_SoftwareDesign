using System.Security.Cryptography;
using System.Text;
using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;

namespace FileAnalysisService.Application.Services;

public class FileAnalysisService : IFileAnalysisService
{
    private readonly IFileAnalysisRepository _repository;

    public FileAnalysisService(IFileAnalysisRepository repository)
    {
        _repository = repository;
    }

    public FileDataHolder AnalyzeAndSave(string content)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
        var hash = BitConverter.ToInt32(hashBytes);

        var wordCount = content.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
        var paragraphCount = content.Split("\n\n").Length;
        var symbolCount = content.Length;

        var fileData = new FileDataHolder(
            id: Guid.NewGuid(),
            hash: hash,
            wordCount: wordCount,
            paragraphCount: paragraphCount,
            symbolCount: symbolCount
        );

        _repository.Add(fileData);
        return fileData;
    }

    public FileDataHolder? GetAnalysisResult(Guid fileId)
    {
        return _repository.Get(fileId);
    }

    public bool DeleteAnalysisResult(Guid fileId)
    {
        return _repository.Delete(fileId) != null;
    }
}