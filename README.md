# mkw-brstm-creator
This utility automatically generates MKWii BRSTMs from youtube links, or FFmpeg compatible audio files (most common extensions work). Made using [NAudio](https://github.com/naudio/NAudio) (multi-channel handling), [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) (youtube download feature), [FFmpeg.NET](https://github.com/cmxl/FFmpeg.NET) (ffmpeg wrapper for C#), and [VGAudio](https://github.com/Thealexbarney/VGAudio) (WAV to BRSTM conversion). 

## Dependencies
The ffmpeg.exe binary needs to be located at **C:\Program Files (x86)\ffmpeg\bin\ffmpeg.exe**. That's the only dependency. You can download FFmpeg [here](https://www.gyan.dev/ffmpeg/builds/).

## Usage
I recommend running the project with `dotnet run`, or using the binary in the releases section. Afterwards, select the track you'd like to create a BRSTM for and paste a youtube link to your song when prompted. The resulting BRSTMs will be placed in `.\brstms`. 

The application will allow you to increase the volume by a factor of 0-10 dB and speed up the song for the final lap, using a multiplier from 1.00-1.30 inclusive. You can also cut the song when prompted.

[Here](https://youtu.be/dDkJ6SXd_gA) is a short video on installation and usage. 
