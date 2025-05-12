using FileStorageService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Repos;

public class FileStoringDbContext : DbContext
{
    protected readonly IConfiguration _configuration;

    public FileStoringDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public DbSet<FileHolder> FileHolders { get; set; } = null!;

    public FileStoringDbContext()
    {
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("FileStoringDataBase"));
    }
}