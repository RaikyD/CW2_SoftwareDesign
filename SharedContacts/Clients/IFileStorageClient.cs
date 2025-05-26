namespace SharedContacts.Clients;

public interface IFileStorageClient
{
    Task<string> GetFileContentAsync(Guid fileId);
    Task<Guid> UploadAsync(Stream content, string fileName);
} 