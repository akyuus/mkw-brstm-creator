# mkw-brstm-creator
This utility automatically generates MKWii BRSTMs from youtube links, or FFmpeg compatible audio files (most common extensions work). Made using [NAudio](https://github.com/naudio/NAudio) (multi-channel handling), [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) (youtube download feature), [FFmpeg.NET](https://github.com/cmxl/FFmpeg.NET) (ffmpeg wrapper for C#), and [VGAudio](https://github.com/Thealexbarney/VGAudio) (WAV to BRSTM conversion). 

## Dependencies
You need to have [ffmpeg](https://ffmpeg.org/download.html) in your system path for the application to work. Additionally, the ffmpeg.exe binary needs to be located at **C:\Program Files (x86)\ffmpeg\bin\ffmpeg.exe**.

A short guide on how to add ffmpeg to your path can be found [here](https://github.com/komaano/mkw-brstm-creator/blob/main/FFMPEG_INSTRUCTIONS.txt) courtesy of Serena.

## Usage
I recommend running the project with `dotnet run`, or using the binary in the releases section. Afterwards, select the track you'd like to create a BRSTM for and paste a youtube link to your song when prompted. The resulting BRSTMs will be placed in `.\brstms`. 

The application will allow you to increase the volume by a factor of 0-10 dB and speed up the song for the final lap, using a multiplier from 1.05-1.30 inclusive. I plan on adding additional customization features in the future. 
