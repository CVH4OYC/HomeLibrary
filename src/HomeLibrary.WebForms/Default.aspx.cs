using System;
using System.Web.UI;

namespace HomeLibrary.WebForms
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Books/List.aspx"));
        }
    }
}
