using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using VGAudio;
using VGAudio.Formats;
using VGAudio.Containers;
using VGAudio.Containers.Wave;
using VGAudio.Containers.NintendoWare;
using NAudio;
using NAudio.Wave;

namespace brstm_maker 
{
    class AudioHandler
    {
        public static void convertToBrstm(string path) 
        {
            using (FileStream fs = File.OpenRead(path)) 
            {
                string newpath = new string(path.Take(path.Length-3).ToArray());
                newpath += "brstm";
                AudioData audio = new WaveReader().Read(fs); 
                byte[] brstmFile = new BrstmWriter().GetFile(audio);
                File.WriteAllBytes(newpath, brstmFile);
                Console.WriteLine($"Done converting {path} to brstm");
            }
        }

        public static string adjustChannels(string path, int outputChannels) 
        {
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + ".wav";

            if(outputChannels == 2) 
            {
                File.Move(path, newpath);
                File.Delete(path);
                return newpath;
            }
            

            using(var input = new WaveFileReader(path)) 
            {
                var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input }, outputChannels);
                WaveFileWriter.CreateWaveFile(newpath, waveProvider);
            }
            
            File.Delete(path);
            return newpath;
        }

        public static string adjustVolume(string path, int dBnum) 
        {
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + "_temp2.wav";;

            ProcessStartInfo processInfo = new ProcessStartInfo("ffmpeg")
            {
                ArgumentList = {
                    "-i",
                    path,
                    "-filter:a",
                    $"volume={dBnum}dB",
                    newpath
                }
            };
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
            File.Delete(path);
            Console.WriteLine(newpath);
            return newpath;
        }
        public static string finalLapMaker(string path) 
        {
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + "_f.wav";;
            if(Char.IsUpper(path[indexof+1])) 
            {
                newpath = path.Substring(0, indexof) + "_F.wav";
            }
            

            ProcessStartInfo processInfo = new ProcessStartInfo("ffmpeg")
            {
                ArgumentList = {
                    "-i",
                    path,
                    "-filter:a",
                    "atempo=1.15",
                    "-vn",
                    newpath
                }
            };
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();

            return newpath;
        }
    }
}