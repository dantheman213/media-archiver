# JackTheVideoRipper

Download videos and audio from YouTube and hundreds of more streaming providers. Designed and developed for Windows 10.

![](https://github.com/dantheman213/JackTheVideoRipper/raw/master/docs/demo.gif)

## Download

Download the latest version in the [release](https://github.com/dantheman213/JackTheVideoRipper/releases) section.

## Features

* Download video and/or audio with wide selection of supported formats

* Extract URLs from any document type or YouTube playlist and add URLs to queue for batch download

* 100+ supported streaming providers

## How Does It Work?

`JackTheVideoRipper` is a GUI that manages and automates powerful tools under-the-hood to provide a streamlined, turn-key, point-and-click experience in order to download video, audio, or playlists in a friendly and easy way. The true power lies within included command-line tools `youtube-dl` and `ffmpeg` which do the heavy lifting of extracting and/or transcoding the media.

## Requirements

* Windows 10 (all editions)

### Bundled Dependencies

All required dependencies are installed, updated, and maintained automatically for you.

* [vcredist-x86](https://www.microsoft.com/en-us/download/confirmation.aspx?id=5555)
* [yt-dlp](https://github.com/yt-dlp/yt-dlp)
* [ffmpeg](https://www.ffmpeg.org/download.html#build-windows)
* [AtomicParsley](http://atomicparsley.sourceforge.net)

## Develop

It is recommended that you [download and run the Windows Installer](https://github.com/dantheman213/JackTheVideoRipper/releases) so that all dependencies are installed correctly before you download the source code, build, and run the application. If the dependencies are not installed when the application runs, the application will crash or not behave correctly.

## FAQs

##### What streaming providers are supported?

A list of support services is available within the app; navigate to the menu and go to Help > About. `yt-dlp` will dictate which services are supported.

##### The downloaded file doesn't play on my computer, what's wrong?

Try using a different media player. The best free media player out there is [VLC Player](https://www.videolan.org/vlc/index.html). Install and use that to ensure you can playback all desired media files.

##### I want to convert the media to another format, how do I do that?

The best free easy-to-use GUI tool is [Handbrake](https://handbrake.fr/).

##### Feature X isn't working correctly, why not?

This app is currently in pre-release and not everything will work correctly. Please standby for further updates.

##### Why is `yt-dlp` not bundled but rather downloaded when Windows Installer runs?

This has to do with `yt-dlp`'s features and functionality. `yt-dlp` is constantly evolving and updating to make sure it is working with the latest changes made by streaming providers. Because of the ever changing nature bundling the current version of `yt-dlp` doesn't make sense. It will likely be out of date already by the time a user runs the installer. Most other software, like bundled apps `vcredist-x86` or `ffmpeg`, are extremely stable and do not need to be altered frequently to work correctly.

##### Why are you using `yt-dlp` over `youtube-dl`?

`youtube-dl` isn't being actively developed. The last release, as of this writing, was in December of 2021. `yt-dlp` is an excellent fork of `youtube-dl` with lots of updates and bug fixes.