using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using HomeLibrary.WebForms.Data;

namespace HomeLibrary.WebForms.Books
{
    public partial class List : Page
    {
        private readonly BookRepository _repository = new BookRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            var search = SearchBox.Text;
            var list = _repository.GetAll(search);
            BooksGrid.DataSource = list;
            BooksGrid.DataBind();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void ResetButton_Click(object sender, EventArgs e)
        {
            SearchBox.Text = string.Empty;
            BindGrid();
        }

        protected void BooksGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteBook")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                _repository.Delete(id);
                MessageLiteral.Text = "<div class=\"alert alert-success alert-dismissible fade show\" role=\"alert\">Книга удалена.<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\"></button></div>";
                BindGrid();
            }
        }
    }
}
