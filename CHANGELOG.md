# v0.9.0

* Fixed issue with yt-dlp not auto updating
* Added dialog when yt-dlp is automatically updated
* Moved Settings into a centralized place and out of WinForms
* Add transcoding options to media ingestion
* Transcoding status update fixes
* Add visual progress bar for download and transcoding
* Remove old separate ingestion dialog choice
* Updated app icon
* Added Get Command feature to list after media ingestion has started
* Added write subs to file option
* Updated authentication section UI in ingestion dialog
* Fix uninstaller missing some files'
* Add video thumbnail as icon in list items when downloading or transcoding
* Fixed issue with media not opening when double clicking item after it's completed
* Fixed update check issue when app starts up
* Use workspace area for files still being downloaded or transcoded
* Improved media ingestion format selection UI/UX

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