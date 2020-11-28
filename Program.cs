using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace brstm_maker
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Select a track:");
            
            foreach(KeyValuePair<string, string> kvp in Tracks.trackNames)
            {
                Console.WriteLine(kvp.Key);
            }

            Console.WriteLine("--------");
            string userSelection = Console.ReadLine();
            string trackFilename = "";
            try 
            {
                trackFilename = Tracks.getFilename(userSelection);
            }
            catch(System.ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            Console.WriteLine("Enter a YouTube URL: ");
            string url = Console.ReadLine();
            string path = await Task.Run(() => YoutubeHandler.downloadAudio(url, trackFilename));
            path = AudioHandler.adjustVolume(path, 8);
            path = AudioHandler.adjustChannels(path, Tracks.getChannelCount(userSelection));
            string finalpath = AudioHandler.finalLapMaker(path);
            AudioHandler.convertToBrstm(path);
            AudioHandler.convertToBrstm(finalpath);
        }
    }
}
