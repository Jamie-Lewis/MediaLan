using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace MediaLan
{
    public static class EngineExtensions
    {
        public static void StartMediaLan(this System.Web.Routing.RouteCollection routes)
        {
            Engine.Startup();
            routes.RouteExistingFiles = true;
            routes.MapPageRoute("Library", "Library", "~/Library.aspx");
            routes.MapPageRoute("Player", "Library/{directory}", "~/Player.aspx");
            routes.MapPageRoute("Mgr", "Manage", "~/Manage.aspx");
            routes.MapPageRoute("Root", "", "~/Login.aspx");
            routes.MapPageRoute("Logon", "Logon", "~/Login.aspx");
        }
    }

    public partial class Engine
    {
        public static string Path { get; set; }
        public static List<MediaTitle> Library { get; set; }
        public static DateTime LastRefresh { get; set; }
        private static object _lock = new object();

        public class MediaTitle
        {
            public string Path;
            public string Skey;
            public JObject MetaData;
            public bool CanStream;
        }
    }
    
    public partial class Engine
    {
        public static void Startup()
        {
            Path = @"C:\DATA\MEDIA";
            
            var _watcher = new FileSystemWatcher(Path);
            _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
            _watcher.Deleted += new FileSystemEventHandler(_watcher_Changed);
            _watcher.Created += new FileSystemEventHandler(_watcher_Changed);
            _watcher.Renamed += _watcher_Renamed;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = true;

            refreshLibrary();
        }

        private static void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            
        }

        public static void refreshLibrary()
        {
            lock (_lock)
            {
                LastRefresh = DateTime.Now;
                Library = new List<MediaTitle>();
                GetListing().ForEach(x =>
                {
                    var fi = new DirectoryInfo(x);
                    if (fi.Exists)
                    {
                        //Cleanup tag files and images etc.
                        CleanupDirectory(x);

                        //Build library parameters
                        var skey = Convert.ToBase64String(Encoding.UTF8.GetBytes(fi.Name.ToCharArray()));
                        var mpdfile = System.IO.Path.Combine(Path, fi.Name, "STREAM", "Manifest.mpd");
                        var metafile = System.IO.Path.Combine(x, "Metadata.json");
                        var canStream = File.Exists(mpdfile);
                        var title = parseTitle(fi);

                        //Fetch OMDB meta data
                        JObject meta = null;
                        var enc = Encoding.UTF8;
                        if (File.Exists(metafile))
                        {
                            meta = JObject.Parse(File.ReadAllText(metafile, enc));
                        }
                        else
                        {
                            var json = httpGetOMdb(title);
                            File.WriteAllText(metafile, json, enc);
                            meta = JObject.Parse(json);
                        }
                        Library.Add(new MediaTitle { Path = x, Skey = HttpUtility.UrlEncode(skey), MetaData = meta, CanStream = canStream });
                    }
                });
            }
        }

        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("CHANGED, NAME: " + e.Name);
            Console.WriteLine("CHANGED, FULLPATH: " + e.FullPath);
            // Can change program state (set invalid state) in this method.
            // ... Better to use insensitive compares for file names.
            refreshLibrary();
        }

        public static string parseTitle(DirectoryInfo fi)
        {
            var title = "";
            foreach (var c in (fi.Name.Replace(".", " ")).Trim().ToCharArray())
            {
                if (!Char.IsLetter(c) && !Char.IsWhiteSpace(c) && c != '\'')
                {
                    break;
                }
                title += c;
            }
            return title.Trim();
        }
        public static string httpGetOMdb(string title)
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

        public static void CleanupDirectory(string path)
        {
            Directory.GetFiles(path, "*.nfo").ToList().ForEach(File.Delete);
            Directory.GetFiles(path, "*.jpg").ToList().ForEach(File.Delete);
            Directory.GetFiles(path, "*.png").ToList().ForEach(File.Delete);
            Directory.GetFiles(path, "*.txt").ToList().ForEach(File.Delete);
            //Directory.GetFiles(x, "*.json").ToList().ForEach(File.Delete);
        }

        public static List<string> GetListing()
        {
            return Directory.GetDirectories(Path).ToList();
        }
    }
}