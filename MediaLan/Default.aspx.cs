using System;

namespace MediaLan
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect((User.Identity != null ? "/Library" : "/Logon"));
        }
    }
}