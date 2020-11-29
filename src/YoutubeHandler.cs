using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using System.IO;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

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
        public async static Task<string> downloadAudio(string id, string trackFilename)
        {

            string current = System.AppContext.BaseDirectory;
            
            if(!Directory.GetCurrentDirectory().Contains("system32"))
            {
                current = Directory.GetCurrentDirectory() + "\\";
            }

            if(!Directory.Exists(current + $"brstms)"))
            {
                Directory.CreateDirectory(current + "brstms");
            }

            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(id);
            var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();
            string path = current + $"brstms\\{trackFilename}_temp.{streamInfo.Container}";

            if(File.Exists(path)) {
                File.Delete(path);
            }
            
            if (streamInfo != null)
            {
                // Get the actual stream
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                // Download the stream to file
                Console.WriteLine("Downloading...");
                await youtube.Videos.Streams.DownloadAsync(streamInfo, path);
                Console.WriteLine($"Downloaded to {path}");
            }

            return path;
        }

    }
}