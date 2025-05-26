using FileAnalysisService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.Repos;

public class FileAnalysisDbContext : DbContext
{
    protected readonly IConfiguration _configuration;

    public FileAnalysisDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public DbSet<FileDataHolder> FilesData { get; set; } = null!;

    public FileAnalysisDbContext()
    {
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("FileAnalysisDatabase"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileDataHolder>(entity =>
        {
            entity.HasKey(e => e.FileId);
            entity.ToTable("FilesData");
            entity.Property(e => e.Hash).HasColumnName("hash").IsRequired();
            entity.Property(e => e.ParagraphCount).HasColumnName("paragraphs").IsRequired();
            entity.Property(e => e.WordCount).HasColumnName("words").IsRequired();
            entity.Property(e => e.SymbolCount).HasColumnName("symbols").IsRequired();
        });
    }
}