namespace SharedContacts.DTOs;

public class FileAnalysisResult
{
    public int Hash { get; set; }
    public int ParagraphCount { get; set; }
    public int WordCount { get; set; }
    public int SymbolCount { get; set; }
}