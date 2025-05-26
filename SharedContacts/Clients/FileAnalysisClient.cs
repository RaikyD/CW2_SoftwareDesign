using System.Net;
using System.Net.Http.Json;
using SharedContacts.DTOs;

namespace SharedContacts.Clients;

public class FileAnalysisClient : IFileAnalysisClient
{
    private readonly HttpClient _httpClient;
    public FileAnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<FileAnalysisResult?> GetStatsAsync(Guid fileId)
    {
        var resp = await _httpClient.GetAsync($"/api/analysis/{fileId}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<FileAnalysisResult>();
    }

    public async Task<int> AnalyzeHashAsync(string content)
    {
        var response = await _httpClient.GetAsync($"api/analysis/GetHash/{content}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }
}