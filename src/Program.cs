using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace brstm_maker
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                AudioHandler.initializeEngine();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey(intercept: true);
                Environment.Exit(1);
            }
            Console.WriteLine("*** AUTO BRSTM CREATOR V1 ***\n");
            Console.WriteLine("Select a track:\n--------------------");

            foreach(KeyValuePair<string, string> kvp in Tracks.trackNames)
            {
                Console.WriteLine($"| {kvp.Key}");
            }

            Console.WriteLine("--------------------");
            Console.Write("Track: ");
            string userSelection = Console.ReadLine();
            string trackFilename = "";
            try 
            {
                trackFilename = Tracks.getFilename(userSelection);
            }
            catch(System.ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept: true);
                Environment.Exit(1);
            }

            string current = System.AppContext.BaseDirectory;
            
            if(!Directory.GetCurrentDirectory().Contains("system32"))
            {
                current = Directory.GetCurrentDirectory();
            }

            int choice = 0;
            string path = "";
            string finalpath = "";
            Console.WriteLine("Enter an input method:\n [1] Youtube URL\n [2] Audio file");

            try
            {
                choice = Int32.Parse(Console.ReadLine());
                path = (choice == 1) ? await youtubeURLCase(trackFilename) : (choice == 2) ? await audioFileCase(trackFilename, Directory.EnumerateFiles(current).ToList()) : throw new InvalidDataException("Invalid input.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept: true);
                Environment.Exit(1);
            }


            Console.WriteLine("How much do you want to increase the volume? (Enter a value from 0-10 (dB))");
            int decibelIncrease = Int32.Parse(Console.ReadLine());
            path = AudioHandler.adjustVolume(path, decibelIncrease);
            string twochannelpath = path;
            path = AudioHandler.adjustChannels(path, Tracks.getChannelCount(userSelection));

            if(!userSelection.Contains('-'))
            {
                Console.WriteLine("Speed factor for final lap? (Enter a value from 1.00-1.30)");
                double speedFactor = Double.Parse(Console.ReadLine());            
                Console.WriteLine("Would you like to specify a start/end time for the final lap? (Y/N)");

                if(Console.ReadLine().ToLower().Equals("y"))
                {
                    int startTime = 0;
                    int endTime = Int32.MaxValue;
                    Console.WriteLine("Enter a start time in seconds (this is relative to your already-cut song).");
                    startTime = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("Enter an end time in seconds (leave this blank if you want to keep the rest of the song)");
                    string end = Console.ReadLine();
                    if(!string.IsNullOrEmpty(end))
                    {
                        endTime = Int32.Parse(end);
                    }
                    else
                    {
                        endTime = startTime + 300;
                    }
                    try
                    {
                        int indexof = path.LastIndexOf('\\');
                        finalpath = path.Substring(0, indexof) + "\\" + trackFilename.Substring(0, trackFilename.Length-2) + "_f1_.wav"; 
                        File.Copy(twochannelpath, finalpath);
                        finalpath = (await AudioHandler.cutAudio(finalpath, startTime, endTime));
                        finalpath = AudioHandler.adjustChannels(finalpath, Tracks.getChannelCount(userSelection));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey(intercept:true);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    finalpath = path;
                }
                
                finalpath = AudioHandler.finalLapMaker(finalpath, speedFactor);
                Console.WriteLine("Converting...");
                AudioHandler.convertToBrstm(path);
                AudioHandler.convertToBrstm(finalpath);
            }
            else
            {
                AudioHandler.convertToBrstm(path);
            }

            foreach(string file in Directory.EnumerateFiles(@".\brstms"))
            {
                string filecheck = file.ToLower();
                if(filecheck.EndsWith(".wav") && !filecheck.EndsWith($"_n.wav") && !filecheck.EndsWith("_f.wav")) File.Delete(file);
            }
            Console.WriteLine($"Finished. Your brstms are here: {Directory.GetParent(path)}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(intercept:true);
        }

        /// <summary>
        /// Handles the case where a user inputs a YouTube URL.  
        /// </summary>
        /// <remarks>Any URL works as long as the full video ID is included. Also parses timestamps. </remarks>
        /// <returns>The path of the downloaded file.</returns>
        public async static Task<string> youtubeURLCase(string trackFilename)
        {
            Console.WriteLine("Enter a Youtube URL. If you want to include a timestamp, make sure it's at the END of the URL.");
            string input = Console.ReadLine();
            string videoId = input.Split('&')[0].Split('=')[1];
            int startTime = 0;
            int endTime = Int32.MaxValue;
            string path = "";
            try
            {
                path = await YoutubeHandler.downloadAudio(videoId, trackFilename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Something went wrong when trying to get your YouTube link. This could be a problem on their end.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept: true);
                Environment.Exit(1);
            }

            path = await AudioHandler.convertToWav(path);

            if(input.Contains("t="))
            {
                startTime = Int32.Parse(input.Split('=').Last());
                Console.WriteLine("Timestamp detected. Do you want to specify an end time as well? (Y/N)");
                if(Console.ReadLine().ToLower().Equals("y"))
                {
                    Console.WriteLine("Enter an end time in seconds.");
                    endTime = Int32.Parse(Console.ReadLine()); 
                }

                try
                {
                    path = await AudioHandler.cutAudio(path, startTime, endTime);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(intercept:true);
                    Environment.Exit(1);
                }
            }

            return path;
        }
        

        /// <summary>
        /// Handles the case where a user inputs a local file. 
        /// </summary>
        /// <remarks>This supports tab autocompletion.</remarks>
        /// <returns>The path of the downloaded file.</returns>
        public async static Task<string> audioFileCase(string trackFilename, List<string> keys)
        {
            Console.WriteLine("Enter the name of an audio file in the current directory.");
            var builder = new StringBuilder();
            var input = Console.ReadKey(intercept:true);
            
            while(input.Key != ConsoleKey.Enter)
            {
                var currentInput = builder.ToString();
                if(input.Key == ConsoleKey.Tab)
                {
                    
                    var match = keys.FirstOrDefault(item => item != currentInput && item.StartsWith(currentInput, true, CultureInfo.InvariantCulture));
                    
                    if (string.IsNullOrEmpty(match))
                    {
                        input = Console.ReadKey(intercept: true);
                        continue;
                    }

                    ClearCurrentLine();
                    builder.Clear();

                    Console.Write(match);
                    builder.Append(match);
                }

                else
                {
                    if (input.Key == ConsoleKey.Backspace && currentInput.Length > 0)
                    {
                        builder.Remove(builder.Length - 1, 1);
                        ClearCurrentLine();

                        currentInput = currentInput.Remove(currentInput.Length - 1);
                        Console.Write(currentInput);
                    }
                    else
                    {
                        var key = input.KeyChar;
                        builder.Append(key);
                        Console.Write(key);
                    }
                }

                input = Console.ReadKey(intercept:true);
            }
            
            var finalinput = builder.ToString();
            Console.WriteLine('\n');

            if(File.Exists(finalinput))
            {
                string current = System.AppContext.BaseDirectory;

                if(!Directory.GetCurrentDirectory().Contains("system32"))
                {
                    current = Directory.GetCurrentDirectory();
                }

                if(!Directory.Exists(current + "\\brstms"))
                {
                    Directory.CreateDirectory(current + "\\brstms");
                }

                string path = finalinput;

                if(!path.Substring(path.Length-3).Equals("wav"))
                {
                    path = await AudioHandler.convertToWav(path);
                }

                AudioHandler.handleExistingFile(current + "\\brstms\\" + trackFilename + "_temp.wav");
                File.Copy(path, current + "\\brstms\\" + trackFilename + "_temp.wav");
                path = current + "\\brstms\\" + trackFilename + "_temp.wav";

                Console.WriteLine("Would you like to specify a start/end time? (Y/N)");
                
                if(Console.ReadLine().ToLower().Equals("y"))
                {
                    int startTime = 0;
                    int endTime = Int32.MaxValue;
                    Console.WriteLine("Enter a start time in seconds.");
                    startTime = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("Enter an end time in seconds (leave this blank if you want to keep the rest of the song)");
                    string end = Console.ReadLine();
                    if(!string.IsNullOrEmpty(end))
                    {
                        endTime = Int32.Parse(end);
                    }
                    else
                    {
                        endTime = startTime + 300;
                    }

                    try
                    {
                        path = await AudioHandler.cutAudio(path, startTime, endTime);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey(intercept:true);
                        Environment.Exit(1);
                    }
                }

                return path;
            }
            else
            {
                Console.WriteLine("Something went wrong. That file either doesn't exist, or you just inputted garbage.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept:true);
                Environment.Exit(1);
                return "";
            }
        }

        /// <remarks>
        /// https://stackoverflow.com/a/8946847/1188513
        /// </remarks>>
        private static void ClearCurrentLine()
        {
            var currentLine = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLine);
        }
    }
}
