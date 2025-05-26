using System;
using System.Collections.Generic;
using System.Linq;
using FileStorageService.Domain.Entities;
using FileStorageService.Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FileStorageTests
{
    class FileTestFileStoringDbContext : FileStoringDbContext
    {
        public FileTestFileStoringDbContext() : base(new ConfigurationBuilder().Build())
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        }
    }
    public class FileStoringRepositoryTests : IDisposable
    {
        private readonly FileTestFileStoringDbContext _context;
        private readonly FileStoringRepository   _repository;

        public FileStoringRepositoryTests()
        {
            _context    = new FileTestFileStoringDbContext();
            _repository = new FileStoringRepository(_context);
        }

        [Fact]
        public void Get_WithExistingId_ShouldReturnEntity()
        {
            var id     = Guid.NewGuid();
            var holder = new FileHolder(id, "file1.txt", 111, "/tmp/file1.txt");
            _context.FileHolders.Add(holder);
            _context.SaveChanges();

            var result = _repository.Get(id);

            Assert.NotNull(result);
            Assert.Equal("file1.txt", result!.FileName);
        }

        [Fact]
        public void Get_WithNonExistingId_ShouldReturnNull()
        {
            
            var result = _repository.Get(Guid.NewGuid());

            
            Assert.Null(result);
        }

        [Fact]
        public void GetByHash_WithExistingHash_ShouldReturnEntity()
        {
            
            var holder = new FileHolder(Guid.NewGuid(), "file2.txt", 222, "/tmp/file2.txt");
            _context.FileHolders.Add(holder);
            _context.SaveChanges();

            
            var result = _repository.GetByHash(222);

            
            Assert.NotNull(result);
            Assert.Equal(222, result!.Hash);
        }

        [Fact]
        public void GetByHash_WithNonExistingHash_ShouldReturnNull()
        {
            
            var result = _repository.GetByHash(999);

            
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllEntities()
        {
            
            var list = new List<FileHolder>
            {
                new FileHolder(Guid.NewGuid(), "a.txt", 1, "/a"),
                new FileHolder(Guid.NewGuid(), "b.txt", 2, "/b"),
            };
            _context.FileHolders.AddRange(list);
            _context.SaveChanges();

            
            var result = _repository.GetAll();

            
            Assert.NotNull(result);
            Assert.Equal(2, result!.Count);
            Assert.Contains(result, x => x.FileName == "a.txt");
            Assert.Contains(result, x => x.FileName == "b.txt");
        }

        [Fact]
        public void Add_ShouldInsertEntity_AndReturnId()
        {
            
            var holder = new FileHolder(Guid.NewGuid(), "new.txt", 333, "/new");

            
            var returnedId = _repository.Add(holder);

            
            Assert.Equal(holder.Id, returnedId);

            // И проверить, что реально попало в БД
            var fromDb = _context.FileHolders.Find(returnedId);
            Assert.NotNull(fromDb);
            Assert.Equal("new.txt", fromDb!.FileName);
        }

        [Fact]
        public void Delete_WithExistingId_ShouldRemoveAndReturnTrue()
        {
            
            var holder = new FileHolder(Guid.NewGuid(), "todelete.txt", 444, "/del");
            _context.FileHolders.Add(holder);
            _context.SaveChanges();

            
            var result = _repository.Delete(holder.Id);

            
            Assert.True(result);
            Assert.Null(_context.FileHolders.Find(holder.Id));
        }

        [Fact]
        public void Delete_WithNonExistingId_ShouldReturnFalse()
        {
            
            var result = _repository.Delete(Guid.NewGuid());

            
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
