# v0.9.14

* Fixed "ffmpeg not found" downloads on Apple Silicon by installing native arm64 ffmpeg/ffprobe instead of Intel builds
* Windows downloads now match the host CPU (native arm64 on Windows on ARM)
* Linux now installs ffmpeg/ffprobe from the distro's package manager (with a graphical password prompt) instead of failing
* Treat binaries that exist but cannot run (e.g. wrong CPU architecture, missing exec bit) as not found

# v0.9.13

* Track downloads in History as soon as they are added, including in-progress and failed ones
* Fixed a postprocessing error caused by ffmpeg quality arguments being applied to every postprocessor
* Fixed copied yt-dlp commands failing in PowerShell due to unquoted output templates and URLs
* Skip thumbnail embedding for formats yt-dlp cannot convert (e.g. AVIF) instead of failing the download
* Added optional per-download technical logs with open/delete and retention controls in Settings
* Show the default "Add" settings beneath the URL input
* Renamed "Copy yt-dlp Command" to "Copy yt-dlp command"

# v0.9.12

* Sanitize custom file names, removing/replacing characters that are invalid in real filenames
* Added an optional "File Name" field in Configure Download to rename the output file
* Renamed "Download Folder" to "Output Folder" in Configure Download
* Added a Bulk Add mode toggle that queues new URLs instantly using the last used download settings
* Pasting a list of URLs now prompts to add them all at once using the default Add settings
* "Copy yt-dlp Command" now uses the full binary paths (yt-dlp and ffmpeg)

# v0.9.2

TBA

# v0.9.0

* Moved to Rust and Tauri

# v0.8.0

* Upgraded .NET framework to 6.x from 4.x.
* Fixed various bugs in order to improve stability
* Upgraded ffmpeg from 4.2.1 to 5.1
* Including ffprobe as a dependency
* Moved away from youtube-dl to yt-dlp as the former appears to be defunct
* Updated toolbar icons
* Updated documentation
* Fixed a bug that prevented thumbnail to be shown
* Fixed blurry text on some screens with DPI scaling
* Added info to format selector
* Improved issue with ingestion file save dialog
* Added task manager link to help menu and also runs when you double click the status bar
* Fixed bug with get command in media ingestion
* Fixed embed subs option not working
* Added download link for yt-dlp in Help
* Added open depenency folder option in Help