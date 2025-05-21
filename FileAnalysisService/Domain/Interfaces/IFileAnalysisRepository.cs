using FileAnalysisService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace FileAnalysisService.Domain.Interfaces;

public interface IFileAnalysisRepository
{
    FileDataHolder Get(Guid id);
    bool Exists(Guid id);
    FileDataHolder? Delete(Guid id);
    void Add(FileDataHolder file);
}