# mkw-brstm-creator
This utility automatically generates MKWii BRSTMs from youtube links. Made using [NAudio](https://github.com/naudio/NAudio) (multi-channel handling), [NYoutubeDL](https://gitlab.com/BrianAllred/NYoutubeDL) (youtube-dl library for C#), and [VGAudio](https://github.com/Thealexbarney/VGAudio) (WAV to BRSTM conversion). 

## Dependencies
You need to have [youtube-dl](https://youtube-dl.org/) and [ffmpeg](https://ffmpeg.org/download.html) in your system path for the application to work.

## Usage
I recommend running the project with `dotnet run`. Afterwards, select the track you'd like to create a BRSTM for and paste a youtube link to your song when prompted. The resulting BRSTMs will be placed in `.\brstms`. 

By default, the application will increase the volume by **7 dB** and speed up the song by a factor of **1.15** for the final lap. I plan on adding more customization in the future.  
