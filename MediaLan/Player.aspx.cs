using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Routing;

namespace MediaLan
{
    public partial class Player : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var key = (string)RouteData.Values["directory"];
            if (key == null)
            {
                //redirect to library
                Response.Clear();
                Response.Write("STREAMING KEY IS INVALID: " + key);
                Response.End();
                return;
            }
            else
            {
                var skey = Encoding.UTF8.GetString(Convert.FromBase64String(Server.UrlDecode(key)));
                var src = Path.Combine(Engine.Path, skey);
                var mpbfile = src + "\\STREAM\\manifest.mpd";
                var isStreamable = File.Exists(mpbfile);

                if (!isStreamable)
                {
                   ph1.Controls.Add(new LiteralControl("<div class=\"notice\">There is not stream available for this media.<br/>Video player is defaulting to source video file.</div><hr/>"));

                    //if (files.Count() == 1)
                    //{
                    //    var video = new FileInfo(files.First()).FullName.Replace(path, "");
                    //    ph1.Controls.Add(new LiteralControl("<video id=\"videoPlayer\" data-dashjs-player controls><source src=\"/STREAM/" + video.Replace("\\", "/") + "\" type=\"video/mp4\"></video>"));
                    //}
                    //else
                    //{
                    //    files.ForEach(x =>
                    //    {
                    //        var video = new FileInfo(x).FullName.Replace(path, "");
                    //        ph1.Controls.Add(new LiteralControl("<p class=\"notice\">" + video + "</p>"));
                    //    });
                    //}
                }
                else
                {
                    //var files = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mkv") || s.EndsWith(".avi") || s.EndsWith(".mp4")).ToList();

                    var video = new FileInfo(mpbfile).FullName.Replace(Engine.Path, "");
                    ph1.Controls.Add(new LiteralControl(skey + "<hr/><video id=\"videoPlayer\" data-dashjs-player controls><source src=\"/STREAM/" + video.Replace("\\", "/") + "\" type=\"application/dash+xml\"></video>"));
                }
            }
        }
    }
}