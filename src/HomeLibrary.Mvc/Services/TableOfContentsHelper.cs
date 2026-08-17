using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using HomeLibrary.Mvc.Models;

namespace HomeLibrary.Mvc.Services;

/// <summary>
/// Помощник для работы с оглавлением: нормализация, парсинг и генерация
/// структурного XML (&lt;toc&gt;/&lt;chapter&gt;/&lt;section&gt;),
/// пригодного для хранения в столбце типа XML и для XML-запросов.
/// </summary>
public static class TableOfContentsHelper
{
    private static readonly Regex PageTailRegex = new(
        @"[—\-–]?\s*(?:с|стр)\.?\s*(\d+)\s*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    /// <summary>
    /// Разбирает хранимый XML оглавления в список глав/разделов
    /// для структурного редактора. Пустой/невалидный XML -> пустой список.
    /// </summary>
    public static List<TocChapter> ParseChapters(string? storedXml)
    {
        var result = new List<TocChapter>();
        if (string.IsNullOrWhiteSpace(storedXml))
        {
            return result;
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(storedXml);
        }
        catch (XmlException)
        {
            return result;
        }

        if (doc.Root is null)
        {
            return result;
        }

        foreach (var chapterEl in doc.Root.Elements("chapter"))
        {
            var chapter = new TocChapter
            {
                Title = ((string?)chapterEl.Attribute("title") ?? chapterEl.Value).Trim(),
                Page = (string?)chapterEl.Attribute("page")
            };

            foreach (var sectionEl in chapterEl.Elements("section"))
            {
                chapter.Sections.Add(new TocSection
                {
                    Title = ((string?)sectionEl.Attribute("title") ?? sectionEl.Value).Trim(),
                    Page = (string?)sectionEl.Attribute("page")
                });
            }

            result.Add(chapter);
        }

        return result;
    }

    /// <summary>
    /// Собирает структурный XML &lt;toc&gt;/&lt;chapter&gt;/&lt;section&gt;
    /// из списка глав редактора. Пустые главы (без заголовка) пропускаются.
    /// Возвращает null, если ни одной значимой главы нет.
    /// </summary>
    public static string? BuildXml(IEnumerable<TocChapter>? chapters)
    {
        if (chapters is null)
        {
            return null;
        }

        var toc = new XElement("toc");
        var number = 1;

        foreach (var ch in chapters)
        {
            if (string.IsNullOrWhiteSpace(ch.Title))
            {
                continue;
            }

            var chapterEl = new XElement("chapter",
                new XAttribute("number", number++),
                new XAttribute("title", ch.Title.Trim()));
            if (!string.IsNullOrWhiteSpace(ch.Page))
            {
                chapterEl.SetAttributeValue("page", ch.Page.Trim());
            }

            foreach (var sec in ch.Sections ?? new List<TocSection>())
            {
                if (string.IsNullOrWhiteSpace(sec.Title))
                {
                    continue;
                }

                var sectionEl = new XElement("section", new XAttribute("title", sec.Title.Trim()));
                if (!string.IsNullOrWhiteSpace(sec.Page))
                {
                    sectionEl.SetAttributeValue("page", sec.Page.Trim());
                }
                chapterEl.Add(sectionEl);
            }

            toc.Add(chapterEl);
        }

        return toc.HasElements ? toc.ToString(SaveOptions.DisableFormatting) : null;
    }

    /// <summary>
    /// Преобразует XML оглавления в читаемый HTML для страницы просмотра.
    /// Главы (chapter) и разделы (section) выводятся вложенным списком.
    /// Если XML не в структуре toc/chapter, возвращает исходное содержимое как есть.
    /// </summary>
    public static string ToDisplayHtml(string? storedXml)
    {
        if (string.IsNullOrWhiteSpace(storedXml))
        {
            return string.Empty;
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(storedXml);
        }
        catch (XmlException)
        {
            return storedXml;
        }

        var root = doc.Root;
        if (root is null)
        {
            return storedXml;
        }

        var chapters = root.Elements("chapter").ToList();
        if (chapters.Count == 0)
        {
            return ToEditorHtml(storedXml);
        }

        var sb = new StringBuilder();
        sb.Append("<ol class=\"toc-list\">");
        foreach (var chapter in chapters)
        {
            sb.Append("<li>");
            AppendEntry(sb, chapter);

            var sections = chapter.Elements("section").ToList();
            if (sections.Count > 0)
            {
                sb.Append("<ul>");
                foreach (var section in sections)
                {
                    sb.Append("<li>");
                    AppendEntry(sb, section);
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }

            sb.Append("</li>");
        }
        sb.Append("</ol>");

        return sb.ToString();
    }

    private static void AppendEntry(StringBuilder sb, XElement element)
    {
        var title = (string?)element.Attribute("title") ?? element.Value;
        var page = (string?)element.Attribute("page");

        sb.Append(WebUtility.HtmlEncode(title?.Trim() ?? string.Empty));
        if (!string.IsNullOrWhiteSpace(page))
        {
            sb.Append(" <span class=\"text-muted\">— с. ")
              .Append(WebUtility.HtmlEncode(page))
              .Append("</span>");
        }
    }

    /// <summary>
    /// Готовит XML оглавления для показа в HTML-редакторе:
    /// снимает внешний корневой элемент &lt;toc&gt;, возвращая внутренний HTML.
    /// </summary>
    public static string ToEditorHtml(string? storedXml)
    {
        if (string.IsNullOrWhiteSpace(storedXml))
        {
            return string.Empty;
        }

        try
        {
            var doc = XDocument.Parse(storedXml);
            if (doc.Root is null)
            {
                return storedXml;
            }

            var inner = new StringBuilder();
            foreach (var node in doc.Root.Nodes())
            {
                inner.Append(node.ToString(SaveOptions.DisableFormatting));
            }

            return inner.ToString();
        }
        catch (XmlException)
        {
            return storedXml;
        }
    }

    /// <summary>
    /// Запасной метод нормализации HTML в well-formed XML.
    /// </summary>
    public static string? NormalizeForStorage(string? editorHtml)
    {
        if (string.IsNullOrWhiteSpace(editorHtml))
        {
            return null;
        }

        var wrapped = $"<toc>{editorHtml}</toc>";
        if (TryParseXml(wrapped, out var normalized))
        {
            return normalized;
        }

        var settings = new XmlReaderSettings
        {
            ConformanceLevel = ConformanceLevel.Fragment,
            DtdProcessing = DtdProcessing.Ignore
        };

        try
        {
            using var stringReader = new StringReader(wrapped);
            using var reader = XmlReader.Create(stringReader, settings);
            var doc = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
            return doc.ToString(SaveOptions.DisableFormatting);
        }
        catch (XmlException)
        {
            var safe = new XElement("toc", editorHtml);
            return safe.ToString(SaveOptions.DisableFormatting);
        }
    }

    private static bool TryParseXml(string xml, out string? normalized)
    {
        try
        {
            var doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            normalized = doc.ToString(SaveOptions.DisableFormatting);
            return true;
        }
        catch (XmlException)
        {
            normalized = null;
            return false;
        }
    }
}
