namespace FileStorageService.Domain.Entities;

public class FileHolder
{
    public Guid Id { get; init; }
    public string FileName { get; init; }
    public int Hash { get; init; }
    public string FileDirectory { get; init; }

    public FileHolder(Guid id, string name, int hash, string dir)
    {
        Id = id;
        FileName = name;
        Hash = hash;
        FileDirectory = dir;
    }
}