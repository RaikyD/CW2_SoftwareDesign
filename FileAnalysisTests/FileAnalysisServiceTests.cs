using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FileAnalysisService.Application.Services;
using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Interfaces;
using Moq;
using Moq.Protected;
using SharedContacts.Clients;
using Xunit;

namespace FileAnalysisTests
{
    public class FileAnalysisServiceTests
    {
        private readonly Mock<IFileAnalysisRepository> _repoMock;
        private readonly Mock<HttpMessageHandler>      _handlerMock;
        private readonly FileStorageClient             _client;
        private readonly FileAnalysisService.Application.Services.FileAnalysisService           _service;

        public FileAnalysisServiceTests()
        {
            _repoMock = new Mock<IFileAnalysisRepository>();
            
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            _client = new FileStorageClient(httpClient);

            _service = new FileAnalysisService.Application.Services.FileAnalysisService(_repoMock.Object, _client);
        }

        [Fact]
        public void AnalyzeFile_ComputesCorrectStats()
        {
            var content = "Hi all\n\nSecond para";
            
            var result = _service.AnalyzeFile(content);
            
            var expectedWords      = content.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var expectedParas      = content.Split("\n\n").Length;
            var expectedSymbols    = content.Length;
            var expectedHash       = expectedWords * expectedParas + expectedSymbols;

            Assert.Equal(expectedWords,   result.WordCount);
            Assert.Equal(expectedParas,   result.ParagraphCount);
            Assert.Equal(expectedSymbols, result.SymbolCount);
            Assert.Equal(expectedHash,    result.Hash);
        }

        [Fact]
        public async Task GetFileStatsAsync_WhenAlreadyInRepo_ReturnsFromRepo()
        {
            var id     = Guid.NewGuid();
            var holder = new FileDataHolder(id, hash: 1, wordCount: 1, paragraphCount: 1, symbolCount: 1);
            _repoMock.Setup(r => r.Exists(id)).Returns(true);
            _repoMock.Setup(r => r.Get(id)).Returns(holder);
            
            var result = await _service.GetFileStatsAsync(id);
            
            Assert.Same(holder, result);
            _handlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetFileStatsAsync_WhenRepoMissesAndStorageNotFound_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.Exists(id)).Returns(false);

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req => req.RequestUri != null && req.RequestUri.ToString().Contains(id.ToString())),
                  ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));
            
            var result = await _service.GetFileStatsAsync(id);
            
            Assert.Null(result);
            _repoMock.Verify(r => r.Add(It.IsAny<FileDataHolder>()), Times.Never);
        }

        [Fact]
        public async Task GetFileStatsAsync_WhenRepoMissesAndStorageSucceeds_AddsAndReturnsNew()
        {
            var id      = Guid.NewGuid();
            var content = "A B C"; 
            _repoMock.Setup(r => r.Exists(id)).Returns(false);

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content)
                });

            FileDataHolder? added = null;
            _repoMock
                .Setup(r => r.Add(It.IsAny<FileDataHolder>()))
                .Callback<FileDataHolder>(fd => added = fd);
            
            var result = await _service.GetFileStatsAsync(id);
            
            Assert.NotNull(result);
            Assert.Equal(id, result!.FileId);
            Assert.Same(added, result);
            _repoMock.Verify(r => r.Add(It.IsAny<FileDataHolder>()), Times.Once);
        }

        [Fact]
        public void GetAnalysisResult_DelegatesToRepo()
        {
            var id     = Guid.NewGuid();
            var holder = new FileDataHolder(id, 0, 0, 0, 0);
            _repoMock.Setup(r => r.Get(id)).Returns(holder);
            
            var result = _service.GetAnalysisResult(id);
            
            Assert.Same(holder, result);
        }

        [Fact]
        public void DeleteAnalysisResult_DelegatesToRepo()
        {
            var id     = Guid.NewGuid();
            var holder = new FileDataHolder(id, 0, 0, 0, 0);
            _repoMock.Setup(r => r.Delete(id)).Returns(holder);
            Assert.True(_service.DeleteAnalysisResult(id));
            
            _repoMock.Setup(r => r.Delete(id)).Returns((FileDataHolder?)null);
            Assert.False(_service.DeleteAnalysisResult(id));
        }
    }
}
