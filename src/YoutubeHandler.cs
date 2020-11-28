using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using NYoutubeDL;

namespace brstm_maker 
{
    class YoutubeHandler 
    {

        /// <summary>
        ///     downloads the audio from the given url and outputs it to the specified track filename
        /// </summary>
        /// <param name="url"></param>
        /// <param name="trackFilename"></param>
        /// <returns>The path of the file created</returns>
        public static string downloadAudio(string url, string trackFilename)
        {
            string current = Directory.GetCurrentDirectory();
            if(!Directory.Exists(current + $"\\brstms)"))
            {
                Directory.CreateDirectory(current + "\\brstms");
            }

            string path = current + $"\\brstms\\{trackFilename}_temp" + ".wav";
            if(File.Exists(path)) {
                File.Delete(path);
            }
            ProcessStartInfo processInfo = new ProcessStartInfo("youtube-dl")
            {
                ArgumentList = {
                    "-x",
                    "--audio-format",
                    "wav",
                    "-o",
                    $"{current}\\brstms\\{trackFilename}_temp.%(ext)s",
                    url
                }
            };
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            var process = Process.Start(processInfo);
            Console.WriteLine("Downloading...");
            process.WaitForExit();
            process.Close();
            Console.WriteLine("Finished downloading.");
            return path;
        }

    }
}