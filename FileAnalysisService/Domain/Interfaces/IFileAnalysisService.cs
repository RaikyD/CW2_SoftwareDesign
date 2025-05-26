using FileAnalysisService.Domain.Entities;

namespace FileAnalysisService.Domain.Interfaces;

public interface IFileAnalysisService
{
    FileDataHolder AnalyzeFile(string content);
    FileDataHolder? GetAnalysisResult(Guid fileId);
    //public FileDataHolder GetFileStats(Guid id);
    bool DeleteAnalysisResult(Guid fileId);
    
    Task<FileDataHolder?> GetFileStatsAsync(Guid fileId);
}