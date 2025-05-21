using FileAnalysisService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedContacts.DTOs;

namespace FileAnalysisService.Presentation.Controllers;

[ApiController]
[Route("api/analyze")]
public class FileAnalysisController : ControllerBase
{
    private readonly IFileAnalysisService _analysisService;

    public FileAnalysisController(IFileAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    [HttpPost]
    public IActionResult AnalyzeContent([FromBody] FileAnalysisRequest request)
    {
        var result = _analysisService.AnalyzeAndSave(request.Content);
        return Ok(result);
    }

    [HttpGet("{fileId}")]
    public IActionResult GetAnalysis(Guid fileId)
    {
        var result = _analysisService.GetAnalysisResult(fileId);
        return result == null ? NotFound() : Ok(result);
    }
}