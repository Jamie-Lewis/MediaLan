using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaLan
{
    public partial class Streaming : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Clear();
            var file = Request.Url.LocalPath.Replace("/STREAMING/", "");
            var mediafile = @"C:\DATA\MEDIA\" + file.Replace("/", "\\");
            var bin = File.ReadAllBytes(mediafile.Replace(".aspx",".mpd"));
            Response.Write(bin);
            //Response.Write(mediafile);
        }
    }
}