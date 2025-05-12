
namespace SharedContacts.DTOs;

public class FileUploadResponse
{
    public Guid FileId { get; set; }
    public string FileName { get; set; }
    public bool IsNew { get; set; } // Флаг, указывающий новый это файл или существующий
}