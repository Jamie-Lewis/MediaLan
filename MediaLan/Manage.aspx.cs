using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaLan
{
    public partial class Manage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ph1.Controls.Add(new LiteralControl("<h2>Last Refresh: " + Engine.LastRefresh + "</h2>"));
            Engine.Library.Where(x => !x.CanStream).ToList().ForEach(x =>
            {
                ph1.Controls.Add(new LiteralControl("<a href=\"medialan://" + x.Skey + "\">" + x.MetaData["Title"] + "</a><br/>"));
            });
        }
    }
}