# v0.9.18

* Getting Started now names the real dependencies (yt-dlp, ffmpeg/ffprobe, AtomicParsley) and lists the exact source each is installed from instead of the vague "engine" and "media processor" wording
* Moved dependency status, updates, and configuration from its own page into Settings → Dependencies
* "Update yt-dlp" now only appears after a check finds a newer release, shown as a green version label beside the current one
* Added per-dependency path overrides so you can point Media Archiver at your own yt-dlp, ffmpeg, ffprobe, and AtomicParsley executables

# v0.9.17

* yt-dlp is now run with a minimal, controlled PATH (Windows system directories only) instead of inheriting the user's full PATH, so unrelated entries — such as junctions into other volumes — are never traversed
* ffmpeg/ffprobe are passed explicitly via --ffmpeg-location for both metadata and downloads

# v0.9.16

* Fixed downloads failing with "WinError 448: The path cannot be traversed because it contains an untrusted mount point" when an unrelated junction/symlink sits on the system PATH

# v0.9.15

* Harden automatic setup downloads: verify HTTP status, detect interrupted/truncated downloads, and validate archives before extracting
* Surface a clear, actionable error (naming the component and URL) instead of a cryptic "invalid Zip archive: Could not find EOCD"
* Prevent concurrent setup runs from corrupting the same downloaded files

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