using System;
using System.IO;
using System.Threading.Tasks;
using FileStorageService.Application.DTO;
using FileStorageService.Application.Services;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace FileStorageTests
{
    public class FileStoringServiceTests
    {
        private readonly Mock<IFileStoringRepository> _mockRepository;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly FileStoringService _service;

        public FileStoringServiceTests()
        {
            _mockRepository = new Mock<IFileStoringRepository>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _service = new FileStoringService(_mockRepository.Object, _mockEnv.Object);
        }

        [Fact]
        public void GetFile_WithValidId_ReturnsFile()
        {
            var fileId = Guid.NewGuid();
            var expectedFile = new FileHolder(fileId, "test.txt", 123, "path/to/file");
            _mockRepository.Setup(repo => repo.Get(fileId)).Returns(expectedFile);
            
            var result = _service.GetFile(fileId);
            
            Assert.Equal(expectedFile, result);
        }

        [Fact]
        public void GetFile_WithEmptyId_ThrowsArgumentException()
        {
            var emptyId = Guid.Empty;
            
            Assert.Throws<ArgumentException>(() => _service.GetFile(emptyId));
        }

        [Fact]
        public void HashExists_WhenHashExists_ReturnsTrue()
        {
            var hash = 123;
            var file = new FileHolder(Guid.NewGuid(), "test.txt", hash, "path/to/file");
            _mockRepository.Setup(repo => repo.GetByHash(hash)).Returns(file);
            
            var result = _service.HashExists(hash);
            
            Assert.True(result);
        }

        [Fact]
        public void HashExists_WhenHashDoesNotExist_ReturnsFalse()
        {
            var hash = 123;
            _mockRepository.Setup(repo => repo.GetByHash(hash)).Returns((FileHolder)null);
            
            var result = _service.HashExists(hash);
            
            Assert.False(result);
        }

        [Fact]
        public void GetFileIdByHash_WhenHashExists_ReturnsFileId()
        {
            var hash = 123;
            var fileId = Guid.NewGuid();
            var file = new FileHolder(fileId, "test.txt", hash, "path/to/file");
            _mockRepository.Setup(repo => repo.GetByHash(hash)).Returns(file);
            
            var result = _service.GetFileIdByHash(hash);
            
            Assert.Equal(fileId, result);
        }

        [Fact]
        public void AddFile_WhenHashDoesNotExist_AddsNewFile()
        {
            var fileDto = new FileUploadDto { FileName = "test.txt", Content = "test content" };
            var hash = 123;
            _mockRepository.Setup(repo => repo.GetByHash(hash)).Returns((FileHolder)null);
            _mockRepository.Setup(repo => repo.Add(It.IsAny<FileHolder>())).Returns(Guid.NewGuid());
            
            var result = _service.AddFile(fileDto, hash);
            
            _mockRepository.Verify(repo => repo.Add(It.IsAny<FileHolder>()), Times.Once);
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task GetFileContentAsync_ReturnsFileContent()
        {
            var fileHolder = new FileHolder(Guid.NewGuid(), "test.txt", 123, Path.Combine("path", "to", "file"));
            var expectedContent = new byte[] { 1, 2, 3 };
            
            var contentRoot = Directory.GetCurrentDirectory();
            var fullPath = Path.Combine(contentRoot, fileHolder.FileDirectory);
            
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllBytes(fullPath, expectedContent);
            
            var result = await _service.GetFileContentAsync(fileHolder);
            
            Assert.Equal(expectedContent, result);
        }
    }
} 