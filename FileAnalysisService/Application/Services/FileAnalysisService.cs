using System.Net;
using System.Security.Cryptography;
using System.Text;
using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;
using SharedContacts.Clients;

namespace FileAnalysisService.Application.Services;

public class FileAnalysisService : IFileAnalysisService
{
    private readonly IFileAnalysisRepository _repository;
    private readonly IFileStorageClient _client;

    public FileAnalysisService(IFileAnalysisRepository repository, IFileStorageClient client)
    {
        _repository = repository;
        _client = client;
    }
    
    public FileDataHolder AnalyzeFile(string content)
    {
        var wordCount = content.Split(new[] { ' ', '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).Length;
        var paragraphCount = content.Split("\n\n").Length;
        var symbolCount = content.Length;
        var hash = wordCount * paragraphCount + symbolCount;
        var fileData = new FileDataHolder(
            fileId: Guid.NewGuid(),
            hash: hash,
            wordCount: wordCount,
            paragraphCount: paragraphCount,
            symbolCount: symbolCount
        );
        return fileData;
    }
    
    public async Task<FileDataHolder?> GetFileStatsAsync(Guid id)
    {
        if (_repository.Exists(id))
            return _repository.Get(id);

        // 1) Забираем содержимое из FileStorage
        string content;
        try
        {
            content = await _client.GetFileContentAsync(id);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        var stats = AnalyzeFile(content);
        var fileData = new FileDataHolder(id, stats.Hash, stats.WordCount, stats.ParagraphCount, stats.SymbolCount);

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
