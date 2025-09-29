; The name of the installer
!define VERSION "0.3.1"

Name "rawspaw"

; To change from default installer icon:
Icon "Assets\RawSpaw.ico"

; The setup filename
OutFile "bin\rawspaw_${VERSION}_setup.exe"

; The default installation directory
InstallDir $PROGRAMFILES\rawspaw

; Registry key to check for directory (so if you install again, it will
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\rawspaw" "Install_Dir"

RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "rawspaw v${VERSION} (required)"

  SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  ; Put file there (you can add more File lines too)
  File "bin\Release\net8.0\publish\rawspaw.exe"
  ; Wildcards are allowed:
  ; File *.dll
  ; To add a folder named MYFOLDER and all files in it recursively, use this EXACT syntax:
  ; File /r MYFOLDER\*.*
  ; See: https://nsis.sourceforge.io/Reference/File
  ; MAKE SURE YOU PUT ALL THE FILES HERE IN THE UNINSTALLER TOO

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\rawspaw "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\rawspaw" "DisplayName" "rawspaw"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\rawspaw" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\rawspaw" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\rawspaw" "NoRepair" 1
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
  ; Adding RawSpaw to PATH variable:
  ${EnvVarUpdate} $0 "PATH" "A" "HKLM" $INSTDIR
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"
  ; SectionIn RO

  CreateDirectory "$SMPROGRAMS\rawspaw"
  CreateShortcut "$SMPROGRAMS\rawspaw\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortcut "$SMPROGRAMS\rawspaw\rawspaw.lnk" "$INSTDIR\rawspaw.exe" "" "$INSTDIR\rawspaw.exe" 0

SectionEnd

Section "Desktop Shortcut" SectionX
    SetShellVarContext current
    CreateShortCut "$DESKTOP\rawspaw.lnk" "$INSTDIR\rawspaw.exe"
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"

  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\rawspaw"
  DeleteRegKey HKLM SOFTWARE\rawspaw

  ; Remove files and uninstaller
  ; MAKE SURE NOT TO USE A WILDCARD. IF A
  ; USER CHOOSES A STUPID INSTALL DIRECTORY,
  ; YOU'LL WIPE OUT OTHER FILES TOO
  Delete $INSTDIR\rawspaw.exe
  Delete $INSTDIR\uninstall.exe

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\rawspaw\*.*"

  ; Remove directories used (only deletes empty dirs)
  RMDir "$SMPROGRAMS\rawspaw"
  RMDir "$INSTDIR"

SectionEnd