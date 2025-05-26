using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using SharedContacts.DTOs;

namespace FileAnalysisTests;

public class FileAnalysisControllerTests
{
    private readonly Mock<IFileAnalysisService> _mockFileAnalysisService;

    public FileAnalysisControllerTests()
    {
        _mockFileAnalysisService = new Mock<IFileAnalysisService>();
    }

    [Fact]
    public async Task GetAnalysis_WhenFileNotFound_ReturnsBadRequest()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _mockFileAnalysisService
            .Setup(s => s.GetFileStatsAsync(fileId))
            .ReturnsAsync((FileDataHolder)null);

        // Act
        var result = await GetAnalysis(fileId);

        // Assert
        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task GetAnalysis_WhenFileExists_ReturnsStats()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var fileStats = new FileDataHolder(
            fileId: fileId,
            hash: 12345,
            wordCount: 10,
            paragraphCount: 2,
            symbolCount: 50
        );

        _mockFileAnalysisService
            .Setup(s => s.GetFileStatsAsync(fileId))
            .ReturnsAsync(fileStats);

        // Act
        var result = await GetAnalysis(fileId);

        // Assert
        var okResult = Assert.IsType<Ok<FileDataHolder>>(result);
        var returnedStats = okResult.Value;
        Assert.Equal(fileStats.FileId, returnedStats.FileId);
        Assert.Equal(fileStats.Hash, returnedStats.Hash);
        Assert.Equal(fileStats.WordCount, returnedStats.WordCount);
        Assert.Equal(fileStats.ParagraphCount, returnedStats.ParagraphCount);
        Assert.Equal(fileStats.SymbolCount, returnedStats.SymbolCount);
    }

    [Fact]
    public async Task GetHash_ReturnsCalculatedHash()
    {
        // Arrange
        var content = "test content";
        var expectedHash = 42;
        var expectedStats = new FileDataHolder(
            fileId: Guid.NewGuid(),
            hash: expectedHash,
            wordCount: 2,
            paragraphCount: 1,
            symbolCount: 12
        );

        _mockFileAnalysisService
            .Setup(s => s.AnalyzeFile(content))
            .Returns(expectedStats);

        // Act
        var result = await GetHash(content);

        // Assert
        var contentResult = Assert.IsType<ContentHttpResult>(result);
        Assert.Equal(expectedHash.ToString(), contentResult.ResponseContent);
    }

    private async Task<IResult> GetAnalysis(Guid id)
    {
        return await FileAnalysisService.Presentation.Controllers.FileAnalysisApiExtensions
            .HandleGetAnalysis(id, _mockFileAnalysisService.Object);
    }

    private async Task<IResult> GetHash(string content)
    {
        return await FileAnalysisService.Presentation.Controllers.FileAnalysisApiExtensions
            .HandleGetHash(content, _mockFileAnalysisService.Object);
    }
} 