using System;
using System.Collections.Generic;
using System.Web.UI;
using HomeLibrary.WebForms.Data;
using HomeLibrary.WebForms.Helpers;
using HomeLibrary.WebForms.Models;
using Newtonsoft.Json;

namespace HomeLibrary.WebForms.Books
{
    public partial class Edit : Page
    {
        private readonly BookRepository _repository = new BookRepository();

        private int? BookId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : (int?)null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            if (BookId.HasValue)
            {
                HeadingLiteral.Text = "Редактирование книги";
                var book = _repository.GetById(BookId.Value);
                if (book == null)
                {
                    Response.Redirect(ResolveUrl("~/Books/List.aspx"));
                    return;
                }

                LoadForm(book);
            }
            else
            {
                HeadingLiteral.Text = "Новая книга";
                TocJsonHidden.Value = "[]";
            }
        }

        private void LoadForm(Book book)
        {
            TitleBox.Text = book.Title;
            AuthorBox.Text = book.Author;
            YearBox.Text = book.PublicationYear.HasValue ? book.PublicationYear.Value.ToString() : string.Empty;
            PagesBox.Text = book.PageCount.HasValue ? book.PageCount.Value.ToString() : string.Empty;
            PublisherBox.Text = book.Publisher;
            IsbnBox.Text = book.ISBN;
            GenreBox.Text = book.Genre;

            var chapters = TableOfContentsHelper.ParseChapters(book.TableOfContents);
            TocJsonHidden.Value = JsonConvert.SerializeObject(chapters);
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            List<TocChapter> chapters = null;
            if (!string.IsNullOrWhiteSpace(TocJsonHidden.Value))
            {
                try
                {
                    chapters = JsonConvert.DeserializeObject<List<TocChapter>>(TocJsonHidden.Value);
                }
                catch
                {
                    chapters = null;
                }
            }

            var book = new Book
            {
                Title = TitleBox.Text.Trim(),
                Author = AuthorBox.Text.Trim(),
                PublicationYear = ParseNullableInt(YearBox.Text),
                PageCount = ParseNullableInt(PagesBox.Text),
                Publisher = NullIfEmpty(PublisherBox.Text),
                ISBN = NullIfEmpty(IsbnBox.Text),
                Genre = NullIfEmpty(GenreBox.Text),
                TableOfContents = TableOfContentsHelper.BuildXml(chapters)
            };

            int targetId;
            if (BookId.HasValue)
            {
                book.Id = BookId.Value;
                _repository.Update(book);
                targetId = book.Id;
            }
            else
            {
                targetId = _repository.Insert(book);
            }

            Response.Redirect(ResolveUrl("~/Books/Details.aspx?id=" + targetId));
        }

        private static int? ParseNullableInt(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed) ? parsed : (int?)null;
        }

        private static string NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
