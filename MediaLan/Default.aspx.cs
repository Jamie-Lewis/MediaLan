using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaLan
{
    public partial class Default : System.Web.UI.Page
    {
        public Dictionary<string, JObject> Library { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["dash"] != null)
            {
                Dashit(@"C:\DATA\MEDIA\Snowfall\Season1\SNOWFALLS01E01.mp4", "160x90", 250);
                //Response.Redirect("/");
            }
            //return;

            Library = new Dictionary<string, JObject>();

            var path = @"C:\DATA\MEDIA";

            var key = Request.QueryString["key"];
            if (key == null)
            {
                ph1.Controls.Add(new LiteralControl("<ul>"));
                var lst = Directory.GetDirectories(path).ToList();

                lst.ForEach(x =>
                {
                    Directory.GetFiles(x, "*.nfo").ToList().ForEach(File.Delete);
                    Directory.GetFiles(x, "*.jpg").ToList().ForEach(File.Delete);
                    Directory.GetFiles(x, "*.png").ToList().ForEach(File.Delete);
                    Directory.GetFiles(x, "*.txt").ToList().ForEach(File.Delete);
                    //Directory.GetFiles(x, "*.json").ToList().ForEach(File.Delete);
                });

                lst.ForEach(x =>
                {

                    var fi = new DirectoryInfo(x);
                    var title = "";
                    foreach (var c in (fi.Name.Replace(".", " ")).Trim().ToCharArray())
                    {
                        if (!Char.IsLetter(c) && !Char.IsWhiteSpace(c) && c != '\'')
                        {
                            break;
                        }
                        title += c;
                    }
                    title = title.Trim();

                    JObject meta = null;
                    var metafile = Path.Combine(x, "Metadata.json");
                    var enc = Encoding.UTF8;
                    if (File.Exists(metafile))
                    {
                        meta = JObject.Parse(File.ReadAllText(metafile, enc));
                    }
                    else
                    {
                        var json = httpget(title);
                        File.WriteAllText(metafile, json, enc);
                        meta = JObject.Parse(json);
                    }

                    var skey = Convert.ToBase64String(Encoding.UTF8.GetBytes(fi.Name.ToCharArray()));

                    var year = meta["Year"]?.ToString().Trim();

                    ph1.Controls.Add(new LiteralControl("<li onclick=\"document.location='?key=" + skey + "'\">"));
                    if (meta["Poster"].ToString() != "N/A") ph1.Controls.Add(new LiteralControl("<img src=\"" + meta["Poster"] + "\" />")); else { ph1.Controls.Add(new LiteralControl("<div style=\"width:150px;height:175px;margin:0;padding:0;\">&nbsp;</div>")); }
                    ph1.Controls.Add(new LiteralControl("<br/><b>" + title + "</b><br/>" + year + "<hr/>Genre: " + meta["Genre"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("Type: " + meta["Type"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("Rated: " + meta["Rated"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("Released: " + meta["Released"] + "<br/>"));
                    ph1.Controls.Add(new LiteralControl("</li>"));
                });
                ph1.Controls.Add(new LiteralControl("</ul>"));
            }
            else
            {
                var skey = Encoding.UTF8.GetString(Convert.FromBase64String(key));
                var src = Path.Combine(path, skey);
                var files = Directory.GetFiles(src).Where(s => s.EndsWith(".mkv") || s.EndsWith(".avi") || s.EndsWith(".mp4")).ToList();

                if (files.Count() == 1)
                {
                    var video = new FileInfo(files.First()).FullName.Replace(path,"");
                    ph1.Controls.Add(new LiteralControl("<video controls><source src=\"/STREAM/" + video.Replace("\\","/") + "\" type=\"video/mp4\"></video>"));
                }
            }
        }

        public string httpget(string title)
        {
            var url = string.Format("http://www.omdbapi.com/?t={0}&apikey=e91a9c29&qwe=1", title);
            var client = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
            var response = client.DownloadString(url);
            return response;
        }

        public async void Dashit(string input, string resolution, int bitrate)
        {
            //ffmpeg -codec:a libvo_aacenc -ar 44100 -ac 1 -codec:v libx264 -profile:v baseline -level 13 -b:v 2000k output.mp4 -i test.mp4
            //ffmpeg - i input.avi - s 160x90 - c:v libx264 -b:v 250k - g 90 - an input_video_160x90_250k.mp4
            //ffmpeg -i input.avi -c:a aac -b:a 128k -vn input_audio_128k.mp4
            //C:\DATA\Production\MediaLan\ffmpeg\bin>ffmpeg -codec:a aac -ar 44100 -ac 1 -codec:v libx264 -profile:v baseline -level 13 -b:v 2000k output.mp4 -i "C:\DATA\MEDIA\Isle Of Dogs (2018) [WEBRip] [1080p] [YTS.AM]\Isle.Of.Dogs.2018.1080p.WEBRip.x264-[YTS.AM].mp4"

            //MP4Box -dash 10000 -dash-profile live -segment-name output-seg "C:\DATA\Production\MediaLan\ffmpeg\bin\output.mp4"

            var ext = input.Substring(input.LastIndexOf("."));
            var outputname = input.Remove(input.LastIndexOf("."));
            var outputpath = input.Remove(input.LastIndexOf("\\"));
            //Directory.CreateDirectory(outputpath);

            int keyframe = 90;
            Process p1 = null; Process p2 = null;

            var vfname = outputname + "_video_" + resolution + "_" + bitrate + "k" + ".mp4";
            if (!File.Exists(outputpath + "\\" + vfname))
            {
                var video = new ProcessStartInfo()
                {
                    FileName = @"C:\DATA\Production\MediaLan\ffmpeg\bin\ffmpeg",
                    Arguments = "-y -i \"" + input + "\" -s " + resolution + " -c:v libx264 -b:v " + bitrate + "k -g " + keyframe + " -an \"" + vfname + "\"",
                    WorkingDirectory = outputpath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                p1 = Process.Start(video);
            }

            var abitrate = 192;
            var afname = outputname + "_audio_" + abitrate + "k" + ".mp4";
            if (!File.Exists(outputpath + "\\" + afname))
            {
                var audio = new ProcessStartInfo()
                {
                    FileName = @"C:\DATA\Production\MediaLan\ffmpeg\bin\ffmpeg",
                    Arguments = "-y -i \"" + input + "\" -vn -f mp4 -acodec -b:a " + abitrate + "k copy \"" + afname + "\"",
                    WorkingDirectory = outputpath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                p2 = Process.Start(audio);
            }

                while (p1 != null && !p1.HasExited || p2 != null && !p2.HasExited)
            {
                System.Threading.Thread.Sleep(1000);
            }

            if (p1 != null)
            {
                var txt = p1.StandardOutput.ReadToEnd();
                Response.Write("VIDEO: " + txt.Replace("\n","<br/>"));
            }
            if (p2 != null)
            {
                var txt = p2.StandardOutput.ReadToEnd();
                Response.Write("AUDIO: " + txt.Replace("\n", "<br/>"));
            }
            mp4boxit(vfname, afname, outputpath, resolution, keyframe);
        }

        public void mp4boxit(string video, string audio, string outputpath, string resolution, int keyframe)
        {
            //MP4Box -dash 10000 -dash-profile live -segment-name output-seg "C:\DATA\Production\MediaLan\ffmpeg\bin\output.mp4"

            var dash = new ProcessStartInfo()
            {
                FileName = @"C:\Program Files\GPAC\MP4Box.exe",
                Arguments = "-dash 10000 -profile dashavc264:live -bs-switching no -out manifest.mpd \"" + video + "\" \"" + audio + "\"",
                WorkingDirectory = outputpath,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var p1 = Process.Start(dash);
            while (!p1.HasExited) { System.Threading.Thread.Sleep(1000); }
            if (p1 != null)
            {
                var txt = p1.StandardOutput.ReadToEnd();
                Response.Write("MP4BOX: " + txt.Replace("\n", "<br/>"));
            }
        }

    }
}