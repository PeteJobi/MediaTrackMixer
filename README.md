# MediaTrackMixer
Using **Media Track Mixer**, you can extract and/or combine media tracks from multiple input media files. For example, you can combine your .mp4 video file with your .srt subtitle file and save it into a .mkv file, or you can extract .mp3 from a .mp4/.mkv music video, or even convert between .mp4 and .mkv. Best of all, this program keeps the encoding of your media files in tact! Powered by FFMPEG.

![image](https://github.com/user-attachments/assets/f8873127-ff06-4fd3-bc42-5de69cfb581e)
![image](https://github.com/user-attachments/assets/bc3ddd1b-b826-410a-bd04-a4ec96f407cb)

## How to build
You need to have at least .NET 9 runtime installed to build the software. Download the latest runtime [here](https://dotnet.microsoft.com/en-us/download). If you're not sure which one to download, try [.NET 9.0 Version 9.0.203](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-9.0.203-windows-x64-installer)

In the project folder, run the below
```
dotnet publish -c Release -p:Platform=x64 -p:WindowsAppSDKSelfContained=true -p:WindowsPackageType=None
```
When that completes, go to `\bin\Release\net<version>-windows\win-x64` and you'll find the **MediaTrackMixer.exe**.

## Run without building
You can also just download the release builds if you don't wish to build manually. Unfortunately packages created in WinUI 3 have to be signed with a certificate, and certificates sourced from trusted companies cost hundreds of dollars. If you wish to install the package, you'll have to install the certificate first, as described [here](https://github.com/PeteJobi/MediaTrackMixer/releases/tag/cert). You only need to do this once - future updates will not require different certificates.

## Tracks?
Media files are often composed of tracks or streams. _.mp3_ files can only have a single audio track. _.mp4_ files often have video and audio tracks, but can also contain subtitle tracks and even chapters (Chapters are basically bookmarks that mark segments in the media and give the user the ability to jump between them, provided their media player supports it). _.mkv_ files very often contain video, audio and subtitle tracks, and can also store chapters. Then there are files, like _.srt_, that are not quite media files on their own, but can be added to media as tracks. Most modern third-party media players allow you to switch between tracks. For example, a _.mkv_ movie might have two audio tracks - one in English and another in Hindi or whatever. English might be the default track, but if you prefer the Hindi dub, you can easily swich to that via your media player. The same is true for subtitles. A movie can have regular English subtitles and also SDH English subtitles with extra descriptive captions for deaf people (personally, I switch to SDH if it's available). Of course, you can completely disable tracks too. If the video comes with subtitles, and you don't want them, you can choose to watch without any subtitle tracks. Same for audio. And as far as I know, media players will not allow you to play more than one track of a type at once, i.e, you can't have 2 subtitle tracks on at the same time.

Now, it just so happens that, thanks to FFMPEG, we can copy tracks from one media file to another. You can copy the audio track from any video file and save it to _.mp3_. You can remove audio from video, by copying all the tracks except audio to a new media file. You can create your own subtitle files and add them to that short movie you directed. If for some reason, you prefer _.mp4_ to _.mkv_, you can copy all the tracks and chapters from that .mkv to a new .mp4 and lose nothing! The possibilities are endless! 

Note that, unlike many video editing tools that allow you to add text to video, adding a subtitle track to a media file can be undone. Most editing tools just "burn" the text directly onto the video permanently. The text cannot be separated from the video, and cannot be disabled in a media player. The benefit is that the text will show in any media player and the video editor has more control over the appearance of the text. The downside is that the video watcher has no control over it.

## How to use
When you open the program, you will be prompted to upload the input media files from which you wish to extract/combine tracks. Once that is done, you have options to add more media, remove specific media or remove all. Each media will contain one or more tracks.

The colour of a circle represents the media the track is from, and the number in the circle is for identification purposes in the next stage. The text immediately after the icon is the title of the track as retrieved from the media's metadata, and the text after that is the track's codec.

You can click on individual tracks to select them, or you can check/uncheck the box near the media file name to select/deselect all files. There are also options to select all video and audio tracks, or to select all subtitles, in a media. When you're done with your selection, click the "Ready" button to move on to the next stage.

On the next page, you can reorder tracks by dragging. This is useful if, for example, you want to set a default subtitle, in which case, you drag that subtitle above the other subtitles. You can also enter or rename titles of individual tracks (you currently can't do this for chapters) by clicking on the edit button. When you're done with this, click on the "Mix!" button to enter the file name of the output media. Navigate to the folder you want it saved in and click Save.

The file types that this program officially supports are .mkv, .mp4, .mp3 and .srt. But there are no limitations at all to file types, and any media file with tracks may work. Keep in mind that there may be limitations with certain file types - for example, .mp3 files cannot store video or subtitle tracks.
