############################################################################################
#      NSIS Installation Script created by NSIS Quick Setup Script Generator v1.09.18
#               Entirely Edited with NullSoft Scriptable Installation System                
#              by Vlasis K. Barkas aka Red Wine red_wine@freemail.gr Sep 2006               
############################################################################################
!define APP_NAME "JackTheVideoRipper"
!define COMP_NAME "dantheman213"
!define WEB_SITE "https://github.com/dantheman213/JackTheVideoRipper"
!define VERSION "0.8.0"
!define COPYRIGHT "Dan  � 2022"
!define DESCRIPTION "Download media easily with a few point and clicks."
!define LICENSE_TXT "..\LICENSE"
!define MAIN_APP_EXE "JackTheVideoRipper.exe"
!define INSTALL_TYPE "SetShellVarContext current"
!define REG_ROOT "HKCU"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"

!define REG_START_MENU "Start Menu Folder"

var SM_Folder

######################################################################

VIProductVersion  "${VERSION}.0"
VIAddVersionKey "ProductName"  "${APP_NAME}"
VIAddVersionKey "CompanyName"  "${COMP_NAME}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileDescription"  "${DESCRIPTION}"
VIAddVersionKey "FileVersion"  "${VERSION}"

######################################################################

SetCompressor ZLIB
Name "${APP_NAME}"
Caption "${APP_NAME}"
OutFile "${APP_NAME}-${VERSION}-installer.exe"
BrandingText "${APP_NAME}"
XPStyle on
InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "$PROGRAMFILES\JackTheVideoRipper"

######################################################################

!include "MUI.nsh"

!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING

!insertmacro MUI_PAGE_WELCOME

!ifdef LICENSE_TXT
!insertmacro MUI_PAGE_LICENSE "${LICENSE_TXT}"
!endif

!insertmacro MUI_PAGE_DIRECTORY

!ifdef REG_START_MENU
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "JackTheVideoRipper"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${REG_ROOT}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${UNINSTALL_PATH}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${REG_START_MENU}"
!insertmacro MUI_PAGE_STARTMENU Application $SM_Folder
!endif

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${MAIN_APP_EXE}"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

######################################################################

Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer
SetOutPath "$INSTDIR"
File "..\bin\Release\net6.0-windows\JackTheVideoRipper.exe"
File "..\bin\Release\net6.0-windows\Newtonsoft.Json.dll"
File "..\bin\Release\net6.0-windows\System.Management.dll"
File "..\bin\Release\net6.0-windows\JackTheVideoRipper.dll"
File "..\bin\Release\net6.0-windows\JackTheVideoRipper.runtimeconfig.json"
SectionEnd

######################################################################

Section -Additional
SetShellVarContext all
!define INSTDIR_DATA "$APPDATA\${APP_NAME}\bin"
SetOutPath "${INSTDIR_DATA}"
File "deps\AtomicParsley.exe"
File "deps\ffmpeg.exe"
File "deps\ffprobe.exe"
File "deps\vcredist_x86.exe"
File "deps\windowsdesktop-runtime-6.0.8-win-x64.exe"

# Download latest version of youtube-dl for end-user
# TODO: This currently runs in background and silently. On computers with slow or spotty Internet connections may be an issue
# This should wait until finished before continuing...
nsExec::ExecToStack 'powershell.exe -Command "(new-object System.Net.WebClient).DownloadFile(\"https://yt-dl.org/downloads/latest/youtube-dl.exe\", \"${INSTDIR_DATA}\youtube-dl.exe\")"'

# Run external installer
ExecWait "${INSTDIR_DATA}\vcredist_x86.exe"
ExecWait "${INSTDIR_DATA}\windowsdesktop-runtime-6.0.8-win-x64.exe"

SectionEnd

######################################################################

Section -Icons_Reg
SetOutPath "$INSTDIR"
WriteUninstaller "$INSTDIR\uninstall.exe"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateDirectory "$SMPROGRAMS\$SM_Folder"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!insertmacro MUI_STARTMENU_WRITE_END
!endif

!ifndef REG_START_MENU
CreateDirectory "$SMPROGRAMS\JackTheVideoRipper"
CreateShortCut "$SMPROGRAMS\JackTheVideoRipper\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\JackTheVideoRipper\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!endif

WriteRegStr ${REG_ROOT} "${REG_APP_PATH}" "" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayName" "${APP_NAME}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "UninstallString" "$INSTDIR\uninstall.exe"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayVersion" "${VERSION}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "Publisher" "${COMP_NAME}"

!ifdef WEB_SITE
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "URLInfoAbout" "${WEB_SITE}"
!endif
SectionEnd

######################################################################

Section Uninstall
${INSTALL_TYPE}
Delete "$INSTDIR\JackTheVideoRipper.exe"
Delete "$INSTDIR\Newtonsoft.Json.dll"
Delete "$INSTDIR\Newtonsoft.Json.xml"
Delete "$INSTDIR\System.IO.Compression.dll"
Delete "$INSTDIR\System.IO.Compression.ZipFile.dll"
Delete "$INSTDIR\uninstall.exe"
!ifdef WEB_SITE
Delete "$INSTDIR\${APP_NAME} website.url"
!endif

RmDir "$INSTDIR"

!ifndef NEVER_UNINSTALL
Delete "${INSTDIR_DATA}\AtomicParsley.exe"
Delete "${INSTDIR_DATA}\ffmpeg.exe"
Delete "${INSTDIR_DATA}\ffprobe.exe"
Delete "${INSTDIR_DATA}\vcredist_x86.exe"
Delete "${INSTDIR_DATA}\windowsdesktop-runtime-6.0.8-win-x64.exe"
 
RmDir "$TEMP\${APP_NAME}"
!endif

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_GETFOLDER "Application" $SM_Folder
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk"
!endif
Delete "$DESKTOP\${APP_NAME}.lnk"

RmDir "$SMPROGRAMS\$SM_Folder"
!endif

!ifndef REG_START_MENU
Delete "$SMPROGRAMS\JackTheVideoRipper\${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\JackTheVideoRipper\${APP_NAME} Website.lnk"
!endif
Delete "$DESKTOP\${APP_NAME}.lnk"

RmDir "$SMPROGRAMS\JackTheVideoRipper"
!endif

DeleteRegKey ${REG_ROOT} "${REG_APP_PATH}"
DeleteRegKey ${REG_ROOT} "${UNINSTALL_PATH}"
SectionEnd

######################################################################

