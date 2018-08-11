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
            //"640x260", 
            Dashit(@"C:\DATA\MEDIA\Ozzy\Ozzy.mp4", new List<string> { "160x90", null }, 250);
            Console.WriteLine("Finished ...");
            Console.ReadLine();
        }


        public static async void Dashit(string input, List<string> resolutions, int bitrate)
        {
            //ffmpeg -codec:a libvo_aacenc -ar 44100 -ac 1 -codec:v libx264 -profile:v baseline -level 13 -b:v 2000k output.mp4 -i test.mp4
            //ffmpeg - i input.avi - s 160x90 - c:v libx264 -b:v 250k - g 90 - an input_video_160x90_250k.mp4
            //ffmpeg -i input.avi -c:a aac -b:a 128k -vn input_audio_128k.mp4
            //C:\DATA\Production\MediaLan\ffmpeg\bin>ffmpeg -codec:a aac -ar 44100 -ac 1 -codec:v libx264 -profile:v baseline -level 13 -b:v 2000k output.mp4 -i "C:\DATA\MEDIA\Isle Of Dogs (2018) [WEBRip] [1080p] [YTS.AM]\Isle.Of.Dogs.2018.1080p.WEBRip.x264-[YTS.AM].mp4"

            //MP4Box -dash 10000 -dash-profile live -segment-name output-seg "C:\DATA\Production\MediaLan\ffmpeg\bin\output.mp4"

            var ext = input.Substring(input.LastIndexOf("."));
            var outputname = input.Remove(input.LastIndexOf("."));
            outputname = outputname.Substring(outputname.LastIndexOf("\\"));
            var outputpath = input.Remove(input.LastIndexOf("\\")) + "\\STREAM";
            if (Directory.Exists(outputpath) == false) Directory.CreateDirectory(outputpath);


            int keyframe = 90;
            Dictionary<Process, string> p1 = new Dictionary<Process, string>();
            Process p2 = null;

            //EXTRACT AUDIO
            var abitrate = 192;
            var afname = outputname + "_audio_" + abitrate + "k" + ".mp4";
            if (!File.Exists(outputpath + "\\" + afname))
            {
                var audio = new ProcessStartInfo()
                {
                    FileName = @"C:\DATA\Production\MediaLan\ffmpeg\bin\ffmpeg",
                    Arguments = "-y -i \"" + input + "\" -vn -b:a " + abitrate + "k \"" + outputpath + "\\" + afname + "\"",
                    WorkingDirectory = outputpath,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                p2 = Process.Start(audio);
            }

            int indx = 0;
            foreach (var resolution in resolutions)
            {
                indx++;
                var vfname = outputname + "_video_" + (resolution != null ? resolution + "_" : "") + bitrate + "k" + ".mp4";
                if (!File.Exists(outputpath + "\\" + vfname))
                {
                    var video = new ProcessStartInfo()
                    {
                        FileName = @"C:\DATA\Production\MediaLan\ffmpeg\bin\ffmpeg",
                        Arguments = "-n -i \"" + input + "\" -b:v " + bitrate*indx + "k -g " + keyframe + " -an \"" + outputpath + "\\" + vfname + "\"" + (resolution != null ? " -s " + resolution : ""),
                        WorkingDirectory = outputpath,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false
                    };
                    p1.Add(Process.Start(video), afname);
                }

                while (p1.Count(x => !x.Key.HasExited) + (p2 != null && !p2.HasExited ? 1 : 0) > 1) {
                    System.Threading.Thread.Sleep(1000);
                }
            }

            while (p1.Any(x => !x.Key.HasExited) || p2 != null && !p2.HasExited)
            {
                System.Threading.Thread.Sleep(1000);
            }

            mp4boxit(p1, afname, outputpath, keyframe);
        }

        public static void mp4boxit(Dictionary<Process, string> videos, string audio, string outputpath, int keyframe)
        {
            //MP4Box -dash 10000 -dash-profile live -segment-name output-seg "C:\DATA\Production\MediaLan\ffmpeg\bin\output.mp4"
            var args = "-dash 10000 -profile dashavc264:live -out manifest.mpd";
            videos.ToList().ForEach(x =>
            {
                args += " \"" + x.Value + "\"";
            });
            var dash = new ProcessStartInfo()
            {
                FileName = @"C:\Program Files\GPAC\MP4Box.exe",
                Arguments = args + " \"" + audio + "\"",
                WorkingDirectory = outputpath,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };

            var p1 = Process.Start(dash);
            while (!p1.HasExited) { System.Threading.Thread.Sleep(1000); }
        }
    }
}
