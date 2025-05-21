using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;

namespace FileAnalysisService.Infrastructure.Repos;

public class FileAnalysisRepository : IFileAnalysisRepository
{
    private readonly FileAnalysisDbContext _context;

    public FileAnalysisRepository(FileAnalysisDbContext context)
    {
        _context = context;
    }

    public FileDataHolder Get(Guid id)
    {
        return _context.FileDataHolders.FirstOrDefault(f => f.FileId == id);
    }

    public bool Exists(Guid id)
    {
        return _context.FileDataHolders.Any(f => f.FileId == id);
    }

    public FileDataHolder? Delete(Guid id)
    {
        var file = Get(id);
        if (file == null) return null;
        _context.FileDataHolders.Remove(file);
        _context.SaveChanges();
        return file;
    }

    public void Add(FileDataHolder file)
    {
        _context.FileDataHolders.Add(file);
        _context.SaveChanges();
    }
}