namespace FileAnalysisService.Domain.Entities;

public class FileDataHolder
{
    public Guid FileId { get; init; }
    public int Hash { get; init; }          // Хеш содержимого файла
    public int WordCount { get; init; }     // Количество слов
    public int ParagraphCount { get; init; } // Количество абзацев
    public int SymbolCount { get; init; }    // Количество символов

    public FileDataHolder(Guid fileId, int hash, int wordCount, int paragraphCount, int symbolCount)
    {
        FileId = fileId;
        Hash = hash;
        WordCount = wordCount;
        ParagraphCount = paragraphCount;
        SymbolCount = symbolCount;
    }
}