namespace FileStorageService.Domain.Entities;
// OK Vrode
// Maybe change Hash type only to string
public class FileHolder
{
    public Guid Id { get; init; }
    public string FileName { get; init; }
    public int Hash { get; init; }
    public string FileDirectory { get; init; }

    public FileHolder(Guid id, string fileName, int hash, string fileDirectory)
    {
        Id = id;
        FileName = fileName;
        Hash = hash;
        FileDirectory = fileDirectory;
    }
}