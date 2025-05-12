using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Repositories;

public class FileStoringRepository : IFileStoringRepository
{
    private readonly FileStoringDbContext _context;

    public FileStoringRepository(FileStoringDbContext context)
    {
        _context = context;
    }

    public FileHolder? Get(Guid id) => _context.FileHolders.FirstOrDefault(f => f.Id == id);

    public FileHolder? GetByHash(int hash) => _context.FileHolders.FirstOrDefault(f => f.Hash == hash);

    public IReadOnlyList<FileHolder>? GetAll() => _context.FileHolders.ToList();

    public Guid Add(FileHolder fileHolder)
    {
        _context.FileHolders.Add(fileHolder);
        _context.SaveChanges();
        return fileHolder.Id;
    }

    public bool Delete(Guid id)
    {
        var file = Get(id);
        if (file == null) return false;
        
        _context.FileHolders.Remove(file);
        _context.SaveChanges();
        return true;
    }
}