using System.Net.Http.Json;
using SharedContacts.DTOs;

namespace SharedContacts.Clients;

public class FileAnalysisClient
{
    private readonly HttpClient _httpClient;
    public FileAnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<FileAnalysisResult> AnalyzeContentAsync(string content)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/analyze", new { Content = content });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FileAnalysisResult>();
    }
}