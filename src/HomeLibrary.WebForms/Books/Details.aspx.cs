using System;
using System.Web.UI;
using HomeLibrary.WebForms.Data;
using HomeLibrary.WebForms.Helpers;
using HomeLibrary.WebForms.Models;

namespace HomeLibrary.WebForms.Books
{
    public partial class Details : Page
    {
        private readonly BookRepository _repository = new BookRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            int id;
            if (!int.TryParse(Request.QueryString["id"], out id))
            {
                ShowNotFound();
                return;
            }

            var book = _repository.GetById(id);
            if (book == null)
            {
                ShowNotFound();
                return;
            }

            Bind(book);
        }

        private void Bind(Book book)
        {
            TitleLiteral.Text = Server.HtmlEncode(book.Title);
            AuthorLiteral.Text = Server.HtmlEncode(book.Author);
            YearLiteral.Text = book.PublicationYear.HasValue ? book.PublicationYear.Value.ToString() : "—";
            PublisherLiteral.Text = Server.HtmlEncode(book.Publisher ?? "—");
            IsbnLiteral.Text = Server.HtmlEncode(book.ISBN ?? "—");
            PagesLiteral.Text = book.PageCount.HasValue ? book.PageCount.Value.ToString() : "—";
            GenreLiteral.Text = Server.HtmlEncode(book.Genre ?? "—");

            var toc = TableOfContentsHelper.ToDisplayHtml(book.TableOfContents);
            TocLiteral.Text = string.IsNullOrWhiteSpace(toc)
                ? "<span class=\"text-muted\">Оглавление не задано.</span>"
                : toc;

            EditLink.HRef = ResolveUrl("~/Books/Edit.aspx?id=" + book.Id);
        }

        private void ShowNotFound()
        {
            CardPanel.Visible = false;
            NotFoundPanel.Visible = true;
        }
    }
}
