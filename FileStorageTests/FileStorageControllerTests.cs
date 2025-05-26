using System.Text;
using FileStorageService.Application.DTO;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using SharedContacts.Clients;

namespace FileStorageTests;

public class FileStorageControllerTests
{
    private readonly Mock<IFileStoringService> _mockFileStoringService;
    private readonly Mock<IFileAnalysisClient> _mockFileAnalysisClient;

    public FileStorageControllerTests()
    {
        _mockFileStoringService = new Mock<IFileStoringService>();
        _mockFileAnalysisClient = new Mock<IFileAnalysisClient>();
    }

    [Fact]
    public async Task UploadFile_WhenFileIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var emptyFile = new Mock<IFormFile>();
        emptyFile.Setup(f => f.Length).Returns(0);

        // Act
        var result = await UploadFile(emptyFile.Object);

        // Assert
        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task UploadFile_WhenFileHashExists_ReturnsExistingFileId()
    {
        // Arrange
        var content = "test content";
        var existingFileId = Guid.NewGuid();
        var hash = 12345;

        var mockFile = CreateMockFile(content);
        
        _mockFileAnalysisClient
            .Setup(c => c.AnalyzeHashAsync(It.IsAny<string>()))
            .ReturnsAsync(hash);

        _mockFileStoringService
            .Setup(s => s.HashExists(hash))
            .Returns(true);

        _mockFileStoringService
            .Setup(s => s.GetFileIdByHash(hash))
            .Returns(existingFileId);

        // Act
        var result = await UploadFile(mockFile.Object);

        // Assert
        var okResult = Assert.IsType<Ok<Guid>>(result);
        Assert.Equal(existingFileId, okResult.Value);
    }

    [Fact]
    public async Task UploadFile_WhenFileIsNew_CreatesNewFile()
    {
        // Arrange
        var content = "test content";
        var newFileId = Guid.NewGuid();
        var hash = 12345;

        var mockFile = CreateMockFile(content);

        _mockFileAnalysisClient
            .Setup(c => c.AnalyzeHashAsync(It.IsAny<string>()))
            .ReturnsAsync(hash);

        _mockFileStoringService
            .Setup(s => s.HashExists(hash))
            .Returns(false);

        _mockFileStoringService
            .Setup(s => s.AddFile(It.IsAny<FileUploadDto>(), hash))
            .Returns(newFileId);

        // Act
        var result = await UploadFile(mockFile.Object);

        // Assert
        var createdResult = Assert.IsType<Created<Guid>>(result);
        Assert.Equal(newFileId, createdResult.Value);
    }

    [Fact]
    public async Task GetFile_WhenFileNotFound_ReturnsNotFound()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _mockFileStoringService
            .Setup(s => s.GetFile(fileId))
            .Returns((FileHolder)null);

        // Act
        var result = await GetFile(fileId);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public async Task GetFile_WhenFileExists_ReturnsFileResult()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var fileContent = Encoding.UTF8.GetBytes("test content");
        var fileHolder = new FileHolder(fileId, "test.txt", 12345, "/path/to/file");

        _mockFileStoringService
            .Setup(s => s.GetFile(fileId))
            .Returns(fileHolder);

        _mockFileStoringService
            .Setup(s => s.GetFileContentAsync(fileHolder))
            .ReturnsAsync(fileContent);

        // Act
        var result = await GetFile(fileId);

        // Assert
        var fileResult = Assert.IsType<FileContentHttpResult>(result);
        Assert.Equal("application/octet-stream", fileResult.ContentType);
        Assert.Equal(fileId.ToString(), fileResult.FileDownloadName);
    }

    private async Task<IResult> UploadFile(IFormFile file)
    {
        return await FileStorageService.Presentation.Controllers.FileStorageApiExtensions
            .HandleFileUpload(file, _mockFileStoringService.Object, _mockFileAnalysisClient.Object);
    }

    private async Task<IResult> GetFile(Guid id)
    {
        return await FileStorageService.Presentation.Controllers.FileStorageApiExtensions
            .HandleFileDownload(id, _mockFileStoringService.Object);
    }

    private static Mock<IFormFile> CreateMockFile(string content)
    {
        var mockFile = new Mock<IFormFile>();
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(contentBytes);

        mockFile.Setup(f => f.Length).Returns(contentBytes.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        mockFile.Setup(f => f.FileName).Returns("test.txt");

        return mockFile;
    }
}
