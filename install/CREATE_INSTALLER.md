# Creating the installer

* Download [NSIS](https://nsis.sourceforge.io/Main_Page)
* Download Dependencies and place them into deps/
  - [ffmpeg.exe and ffprobe.exe](https://www.ffmpeg.org/download.html#build-windows)
  - [AtomicParsley.exe](http://atomicparsley.sourceforge.net/)
  - [vcredist_x86.exe](https://www.microsoft.com/en-us/download/details.aspx?id=26999)
  - [windowsdesktop-runtime-6.0.8-win-x64.exe](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* Compile and build Release version of app within Visual Studio
* Increment the version within installer.nsi, version file at root, and in AssemblyInfo in *.csproj
* Use NSIS to compile `installer.nsi`
* Installer should be generated within install/