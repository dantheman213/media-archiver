# JackTheVideoRipper

Download videos and audio from YouTube and hundreds of more streaming providers. Designed and developed for Windows 10.

![](https://github.com/dantheman213/JackTheVideoRipper/raw/master/docs/demo.gif)

## Download

Download the latest version in the [release](https://github.com/dantheman213/JackTheVideoRipper/releases) section.

## How Does It Work?

JackTheVideoRipper manages and automates `youtube-dl` and `ffmpeg` under-the-hood to provide a streamlined, turn-key, point-and-click experience in order to download video or audio in a friendly and easy way.

## Requirements

* Windows 10 (all editions)

### Bundled Dependencies

All required dependencies are installed, updated, and maintained automatically for you.

* [vcredist-x86](https://www.microsoft.com/en-us/download/confirmation.aspx?id=5555)
* [youtube-dl](https://github.com/ytdl-org/youtube-dl)
* [ffmpeg](https://www.ffmpeg.org/download.html#build-windows)
* [AtomicParsley](http://atomicparsley.sourceforge.net)

## FAQs

##### The downloaded file doesn't play on my computer, what's wrong?

Try using a different media player. The best free media player out there is [VLC Player](https://www.videolan.org/vlc/index.html). Install and use that to ensure you can playback all desired media files.

##### I want to convert the media to another format, how do I do that?

The best free easy-to-use GUI tool is [Handbrake](https://handbrake.fr/).

##### Feature X isn't working correctly, why not?

This app is currently in pre-release and not everything will work correctly. Please standby for further updates.

##### Why is `youtube-dl` not bundled but rather downloaded when Windows Installer runs?

This has to do with `youtube-dl`'s features and functionality. `youtube-dl` is constantly evolving and updating to make sure it is working with the latest changes made by streaming providers. Because of the ever changing nature bundling the current version of `youtube-dl` doesn't make sense. It will likely be out of date already by the time a user runs the installer. Most other software, like bundled apps `vcredist-x86` or `ffmpeg`, are extremely stable and do not need to be altered frequently to work correctly.
