using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RegURI.RegisterUriScheme();
            string path = @"C:\DATA\MEDIA\";

            if (args != null && args.Count() > 0)
            {
                foreach (var arg in args)
                {
                    Console.WriteLine("Argument: " + arg);
                    var skey = arg.Remove(arg.Length - 1).Replace(RegURI.UriScheme + "://", "");
                    var directory_name = Encoding.UTF8.GetString(Convert.FromBase64String(System.Web.HttpUtility.UrlDecode(skey)));
                    Console.WriteLine("Directory Name: " + directory_name);

                    var srcdir = Path.Combine(path, directory_name);
                    var files = Directory.GetFiles(srcdir, "*.*").Where(x => x.EndsWith(".mkv") || x.EndsWith(".avi") || x.EndsWith(".mp4")).ToList();
                    files.ForEach(x =>
                    {
                        Console.WriteLine(x);
                        Dashify(x, new List<string> { "1920x1080:0", "1280x720:17", "640x40:24" });
                    });
                }
            }

            Console.WriteLine("Finished ...");
            Console.ReadLine();
        }

        public static void PrepareDirectory()
        {
            //1. Rename top level to [TITLE], [YEAR]
            //2. If TV Series, create structure: [Season x]\[S01E01]
            //3. Rename files without any tags etc. and cleanup spaces etc.

        }

        public static void Dashify(string input, List<string> resolutions)
        {
            var ext = input.Substring(input.LastIndexOf("."));
            var outputname = input.Remove(input.LastIndexOf("."));
            outputname = outputname.Substring(outputname.LastIndexOf("\\"));
            var outputpath = input.Remove(input.LastIndexOf("\\")) + "\\STREAM";
            if (Directory.Exists(outputpath) == false) Directory.CreateDirectory(outputpath);

            Process p2 = null;

            //EXTRACT AUDIO
            var abitrate = 125;
            var afname = outputname + "_audio" + ".mp4"; //" + abitrate + "k
            if (!File.Exists(outputpath + "\\" + afname))
            {
                var audio = new ProcessStartInfo()
                {
                    //
                    FileName = @"C:\DATA\Production\MediaLan\ffmpeg\bin\ffmpeg",
                    Arguments = "-n -i \"" + input + "\" -b:a " + abitrate + "k -vn \"" + outputpath + "\\" + afname + "\"",
                    WorkingDirectory = outputpath
                };
                p2 = Process.Start(audio);
            }

            //ENCODE VIDEO FOR EACH RESOLUTION
            int keyframe = 90;
            var existingfiles = new List<string>();
            Dictionary<Process, string> p1 = new Dictionary<Process, string>();
            foreach (var spec in resolutions)
            {
                var tmp = spec.Split(':');
                var resolution = tmp[0];
                var crf = tmp[1];
                
                var vfname = outputname + "_video_" + (resolution != null ? resolution + "_" : "") + crf + ".mp4";
                if (!File.Exists(outputpath + "\\" + vfname))
                {
                    //var rfilename = "\"" + outputpath + "\\" + outputname + "_video_{0}" + ".mp4" + "\"";
                    //var args = "-i \"" + input + "\" -filter_complex '[0:v]yadif,split=3[out1][out2][out3]' \\ -map '[out1]' -s 1280x720 -an " + String.Format(rfilename,"HD") + " \\ -map '[out2]' -s 640x480 -an " + String.Format(rfilename, "SD") + " \\ -map '[out3]' -s 320x240 -an " + String.Format(rfilename, "VGA");
                     var video = new ProcessStartInfo()
                    {
                        FileName = @"C:\DATA\Production\MediaLan\ffmpeg\bin\ffmpeg",
                         //Arguments = args, //-profile:v baseline 
                         //Arguments = "-i \"" + input + "\" -g " + keyframe + " -pix_fmt yuv420p -c:v libvpx-vp9 -an -crf " + crf + " \"" + outputpath + "\\" + vfname + "\" -vf scale=-2:1080,setsar=1", // + (resolution != null ? " -s " + resolution : ""),
                         Arguments = "-i \"" + input + "\" -movflags +faststart -an \"" + outputpath + "\\" + vfname + "\" -crf " + crf, // + (resolution != null ? " -s " + resolution : ""),
                         WorkingDirectory = outputpath,
                        //UseShellExecute = false,
                        //RedirectStandardOutput = true,
                    };
                    p1.Add(Process.Start(video), vfname);
                }
                else
                {
                    existingfiles.Add(vfname);
                }

                while (p1.Count(x => !x.Key.HasExited) + (p2 != null && !p2.HasExited ? 1 : 0) > 1) {
                    System.Threading.Thread.Sleep(1000);
                }
            }

            while (p1.Any(x => !x.Key.HasExited) || p2 != null && !p2.HasExited)
            {
                System.Threading.Thread.Sleep(500);
            }

            mp4boxit(p1, existingfiles, afname, outputpath, keyframe);
        }

        public static void mp4boxit(Dictionary<Process, string> videos, List<string> existingfiles, string audio, string outputpath, int keyframe)
        {
            var args = "-dash 5000 -rap -frag-rap -profile live -bs-switching no -out manifest.mpd";
            videos.ToList().ForEach(x =>
            {
                args += " \"" + outputpath + x.Value + "\"";
            });
            existingfiles.ForEach(x =>
            {
                args += " \"" + outputpath + x + "\"";
            });
            var dash = new ProcessStartInfo()
            {
                FileName = @"C:\Program Files\GPAC\MP4Box.exe",
                Arguments = args + " \"" + outputpath + audio + "\"",
                WorkingDirectory = outputpath,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };

            var p1 = Process.Start(dash);
            while (!p1.HasExited) { System.Threading.Thread.Sleep(500); }
        }
    }
}
