namespace SharedContacts.Clients;

public class FileStorageClient
{
    private readonly HttpClient _httpClient;
    public FileStorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //Получение содержимого файла по ID
    public async Task<string> GetFileContentAsync(Guid fileId)
    {
        var response = await _httpClient.GetAsync($"/api/content/{fileId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    //Проверка существования файла
    public async Task<bool> FileExistsAsync(Guid fileId)
    {
        var response = await _httpClient.GetAsync($"/api/files/{fileId}/exists");
        return response.IsSuccessStatusCode;
    }
}