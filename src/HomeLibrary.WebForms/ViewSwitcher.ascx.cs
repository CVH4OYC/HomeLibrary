using System;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls.Resolvers;

namespace HomeLibrary.WebForms
{
    public partial class ViewSwitcher : System.Web.UI.UserControl
    {
        protected string CurrentView { get; private set; }

        protected string AlternateView { get; private set; }

        protected string SwitchUrl { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var isMobile = WebFormsFriendlyUrlResolver.IsMobileView(new System.Web.HttpContextWrapper(Context));
            CurrentView = isMobile ? "Mobile" : "Desktop";
            AlternateView = isMobile ? "Desktop" : "Mobile";

            var switchView = isMobile ? "Desktop" : "Mobile";
            var url = GetRouteUrl("AspNet.FriendlyUrls.SwitchView", new { view = switchView, __FriendlyUrls_CurrentVirtualPath = Request.FilePath });
            SwitchUrl = url;
        }
    }
}
