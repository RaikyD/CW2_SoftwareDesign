// SharedContacts.Clients/FileStorageClient.cs
using System.Net.Http.Json;

namespace SharedContacts.Clients;

public class FileStorageClient : IFileStorageClient
{
    private readonly HttpClient _httpClient;
    public FileStorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Получение содержимого файла по ID
    public async Task<string> GetFileContentAsync(Guid fileId)
    {
        var response = await _httpClient.GetAsync($"/api/files/storage-request/{fileId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    // Загрузка файла: возвращает сгенерированный ID
    public async Task<Guid> UploadAsync(Stream content, string fileName)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StreamContent(content), "file", fileName);
        var response = await _httpClient.PostAsync("/api/files", form);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}