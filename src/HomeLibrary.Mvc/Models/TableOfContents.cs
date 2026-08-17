namespace HomeLibrary.Mvc.Models;

/// <summary>Глава оглавления для структурного редактора.</summary>
public class TocChapter
{
    public string Title { get; set; } = string.Empty;
    public string? Page { get; set; }
    public List<TocSection> Sections { get; set; } = new();
}

/// <summary>Раздел (подпункт) главы.</summary>
public class TocSection
{
    public string Title { get; set; } = string.Empty;
    public string? Page { get; set; }
}
