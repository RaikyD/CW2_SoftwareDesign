using System;
using System.Linq;
using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FileAnalysisTests
{
    // -------------------------------
    // Тестовый контекст, только InMemory
    // -------------------------------
    public class FileTestFileAnalysisDbContext : FileAnalysisDbContext
    {
        public FileTestFileAnalysisDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            optionsBuilder.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
        }
    }

    // -------------------------------
    // Тесты репозитория
    // -------------------------------
    public class FileAnalysisRepositoryTests : IDisposable
    {
        private readonly FileTestFileAnalysisDbContext _context;
        private readonly FileAnalysisRepository    _repository;

        public FileAnalysisRepositoryTests()
        {
            _context    = new FileTestFileAnalysisDbContext();
            _repository = new FileAnalysisRepository(_context);
        }

        [Fact]
        public void Add_ShouldPersistEntity()
        {
            var id   = Guid.NewGuid();
            var data = new FileDataHolder(id, hash: 100, wordCount: 10, paragraphCount: 2, symbolCount: 50);
            
            _repository.Add(data);
            
            var fromDb = _context.FilesData.Find(id);
            Assert.NotNull(fromDb);
            Assert.Equal(10, fromDb!.WordCount);
            Assert.Equal(2,  fromDb.ParagraphCount);
            Assert.Equal(50, fromDb.SymbolCount);
            Assert.Equal(100,fromDb.Hash);
        }

        [Fact]
        public void Get_WithExistingId_ReturnsEntity()
        {
            var id   = Guid.NewGuid();
            var data = new FileDataHolder(id, hash: 101, wordCount: 5, paragraphCount: 1, symbolCount: 20);
            _context.FilesData.Add(data);
            _context.SaveChanges();
            
            var result = _repository.Get(id);
            
            Assert.NotNull(result);
            Assert.Equal(101, result.Hash);
            Assert.Equal(5,   result.WordCount);
        }

        [Fact]
        public void Exists_WhenPresent_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            _context.FilesData.Add(new FileDataHolder(id, hash: 1, wordCount: 1, paragraphCount: 1, symbolCount: 1));
            _context.SaveChanges();
            
            Assert.True(_repository.Exists(id));
        }

        [Fact]
        public void Exists_WhenAbsent_ReturnsFalse()
        {
            Assert.False(_repository.Exists(Guid.NewGuid()));
        }

        [Fact]
        public void Delete_WhenPresent_RemovesAndReturnsEntity()
        {
            var id   = Guid.NewGuid();
            var data = new FileDataHolder(id, hash: 2, wordCount: 2, paragraphCount: 2, symbolCount: 2);
            _context.FilesData.Add(data);
            _context.SaveChanges();

            
            var deleted = _repository.Delete(id);
            
            Assert.NotNull(deleted);
            Assert.False(_context.FilesData.Any(f => f.FileId == id));
        }

        [Fact]
        public void Delete_WhenAbsent_ReturnsNull()
        {
            
            var result = _repository.Delete(Guid.NewGuid());

            
            Assert.Null(result);
        }

        public void Dispose()
        {
            // Очищаем InMemory БД после каждого теста
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
