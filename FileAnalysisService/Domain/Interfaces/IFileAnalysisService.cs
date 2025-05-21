using FileAnalysisService.Domain.Entities;

namespace FileAnalysisService.Domain.Interfaces;

public interface IFileAnalysisService
{
    FileDataHolder AnalyzeAndSave(string content);
    FileDataHolder? GetAnalysisResult(Guid fileId);
    bool DeleteAnalysisResult(Guid fileId);
}