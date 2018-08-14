using Newtonsoft.Json.Linq;
using System;
using System.Web.Security;
using System.Web.UI;

namespace MediaLan
{
    public class LibraryItem
    {
        public string Path;
        public JObject Metadata;
        public bool CanStream;
    }

    public partial class Library : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ph1.Controls.Add(new LiteralControl("<ul>"));

            Engine.Library.ForEach(x =>
            {
                if (x.CanStream)
                {
                    var meta = x.MetaData;
                    var title = x.MetaData["title"];

                    var year = meta["Year"]?.ToString().Trim();
                    ph1.Controls.Add(new LiteralControl("<li onclick=\"document.location='/Library/" + x.Skey + "'\">"));
                    if (meta["Poster"] != null && meta["Poster"].ToString() != "N/A") ph1.Controls.Add(new LiteralControl("<img src=\"" + meta["Poster"] + "\" />")); else { ph1.Controls.Add(new LiteralControl("<div style=\"width:150px;height:175px;margin:0;padding:0;\">&nbsp;</div>")); }
                    ph1.Controls.Add(new LiteralControl("<br/><b>" + title + "</b><br/>" + year + "<hr/>Genre: " + meta["Genre"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("Type: " + meta["Type"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("Rated: " + meta["Rated"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("Released: " + meta["Released"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("</li>"));

                }
            });
            ph1.Controls.Add(new LiteralControl("</ul>"));
        }

        protected void Signout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("/Logon");
        }
    }
}