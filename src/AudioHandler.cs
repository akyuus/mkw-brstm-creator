using System;
using System.Threading.Tasks;
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
using FFmpeg.NET;

namespace brstm_maker 
{
    class AudioHandler
    {
        private static readonly Engine ffmpeg = new Engine("C:\\Program Files (x86)\\ffmpeg\\bin\\ffmpeg.exe");
        public static void convertToBrstm(string path) 
        {
            WaveStructure structure;
            WaveReader reader = new WaveReader();
            byte[] fs = File.ReadAllBytes(path);
            string newpath = new string(path.Take(path.Length-3).ToArray());
            newpath += "brstm";
            handleExistingFile(newpath);
            using(FileStream stream = File.OpenRead(path))
            {
                structure = reader.ReadMetadata(stream);
            }
            AudioData audio = reader.Read(fs); 
            audio.SetLoop(true, 0, structure.SampleCount);
            byte[] brstmFile = new BrstmWriter().GetFile(audio);
            File.WriteAllBytes(newpath, brstmFile);
            Console.WriteLine($"Converted: {path} \n       --> {newpath}");
        }

        public async static Task<string> convertToWav(string path) 
        {
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + "_temp2.wav";
            handleExistingFile(newpath);
            var inputFile = new MediaFile(path);
            var outputFile = new MediaFile(newpath);

            await ffmpeg.ConvertAsync(inputFile, outputFile);
            Console.WriteLine($"Converted: {path} \n       --> {newpath}");
            File.Delete(path);
            return newpath;
        }
        public static string adjustChannels(string path, int outputChannels) 
        {
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + ".wav";
            if(outputChannels == 2) 
            {
                File.Copy(path, newpath);
                return newpath;
            }
            

            using(var input = new WaveFileReader(path)) 
            {
                var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input }, outputChannels);
                WaveFileWriter.CreateWaveFile(newpath, waveProvider);
            }
            
            return newpath;
        }

        public static string adjustVolume(string path, int dBnum) 
        {
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + "_tempVOL.wav";
            ProcessStartInfo processInfo = new ProcessStartInfo("C:\\Program Files (x86)\\ffmpeg\\bin\\ffmpeg.exe")
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
            return newpath;
        }

        /// <summary>
        /// Cuts a wav file using given start and end times.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <param name="startTime">Start time in seconds.</param>
        /// <param name="endTime">End time in seconds.</param>
        /// <returns>The new file path.</returns>
        public async static Task<string> cutAudio(string path, int startTime, int endTime = Int32.MaxValue)
        {
            var duration = getMediaFileDuration(path);
            if(endTime > duration)
            {
                endTime = duration-1;
            }
            if(startTime > endTime) throw new InvalidDataException(message: "ERROR: Start time cannot be greater than end time.");
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + "_cut.wav";
            var inputFile = new MediaFile(path);
            var outputFile = new MediaFile(newpath);
            var options = new ConversionOptions();
            options.CutMedia(TimeSpan.FromSeconds(startTime), TimeSpan.FromSeconds(endTime - startTime));
            await ffmpeg.ConvertAsync(inputFile, outputFile, options);
            Console.WriteLine($"Cut {path} @ {startTime} seconds\n--> {newpath}");
            return newpath;
        }
        public static string finalLapMaker(string path, double factor) 
        {
            
            
            int indexof = path.LastIndexOf('_');
            string newpath = path.Substring(0, indexof) + "_f.wav";
            if(Char.IsUpper(path[indexof+1])) 
            {
                newpath = path.Substring(0, indexof) + "_F.wav";
            }
            
            
            ProcessStartInfo processInfo = new ProcessStartInfo("C:\\Program Files (x86)\\ffmpeg\\bin\\ffmpeg.exe")
            {
                ArgumentList = {
                    "-i",
                    path,
                    "-filter:a",
                    $"atempo={factor}",
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

        /// <summary>
        /// Gets the file duration in seconds.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>An int that represents the end time in seconds.</returns>
        public static int getMediaFileDuration(string path)
        {
            byte[] allBytes = File.ReadAllBytes(path);
            int byterate = BitConverter.ToInt32(new[] { allBytes[28], allBytes[29], allBytes[30], allBytes[31] }, 0);
            int duration = (allBytes.Length - 8) / byterate;
            return duration;
        }
        public static void handleExistingFile(string path)
        {
            if(File.Exists(path)) File.Delete(path);
        }
    }
}