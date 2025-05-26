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
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("FileStorageDatabase"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileHolder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("FileHolders");
            entity.Property(e => e.Hash).HasColumnName("hash").IsRequired();
            entity.Property(e => e.FileName).HasColumnName("fileName").IsRequired();
            entity.Property(e => e.FileDirectory).HasColumnName("fileDirectory").IsRequired();
        });
    }
}