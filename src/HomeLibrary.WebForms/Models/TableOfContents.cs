using System.Collections.Generic;

namespace HomeLibrary.WebForms.Models
{
    /// <summary>Глава оглавления для структурного редактора.</summary>
    public class TocChapter
    {
        public string Title { get; set; }
        public string Page { get; set; }
        public List<TocSection> Sections { get; set; }

        public TocChapter()
        {
            Title = string.Empty;
            Sections = new List<TocSection>();
        }
    }

    /// <summary>Раздел (подпункт) главы.</summary>
    public class TocSection
    {
        public string Title { get; set; }
        public string Page { get; set; }

        public TocSection()
        {
            Title = string.Empty;
        }
    }
}
