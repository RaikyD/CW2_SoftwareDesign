namespace SharedContacts.DTOs;

public class FileAnalysisResult
{
    public int Hash { get; init; }          // Хеш содержимого файла
    public int WordCount { get; init; }     // Количество слов
    public int ParagraphCount { get; init; } // Количество абзацев
    public int SymbolCount { get; init; }    // Количество символов

}