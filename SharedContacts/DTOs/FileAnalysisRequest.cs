namespace SharedContacts.DTOs;


public class FileAnalysisRequest
{
    
    public string Content { get; set; } // Содержимое файла для анализа
    public Guid? FileId { get; set; }   // Опционально: ID файла, если он уже сохранён в FileStorageService
}