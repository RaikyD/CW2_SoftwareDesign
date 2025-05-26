using SharedContacts.DTOs;

namespace SharedContacts.Clients;

public interface IFileAnalysisClient
{
    Task<FileAnalysisResult?> GetStatsAsync(Guid fileId);
    Task<int> AnalyzeHashAsync(string content);
} 