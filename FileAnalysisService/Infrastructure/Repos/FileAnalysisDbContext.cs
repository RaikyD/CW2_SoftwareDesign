using FileAnalysisService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.Repos;

public sealed class FileAnalysisDbContext : DbContext
{
    public FileAnalysisDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<FileDataHolder> FileDataHolders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileDataHolder>(e =>
        {
            e.HasKey(x => x.FileId);
            e.ToTable("file_analysis_results"); 
        });
    }
}