; The name of the installer
!define VERSION "0.4.0"

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
  
  ; Check for write access
  EnVar::Check "NULL" "NULL"
  Pop $0
  DetailPrint "EnVar::Check write access HKCU returned=|$0|"
  
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
  
SectionEnd


Section "Add rawspaw to PATH"
    SetShellVarContext current
    ; TODO: Add PATH
SectionEnd


Section "Add rawspaw to Windows Explorer context menu"
    SetShellVarContext current
    ; CreateShortCut "$DESKTOP\rawspaw.lnk" "$INSTDIR\rawspaw.exe"
    WriteRegStr HKLM "Software\Classes\directory\shell\rawspaw" "" "Raw-Spaw"
    WriteRegStr HKLM "Software\Classes\directory\shell\rawspaw" "Icon" "$INSTDIR\rawspaw.exe"
    WriteRegStr HKLM "Software\Classes\directory\shell\rawspaw\command" "" '$INSTDIR\rawspaw.exe -i -w "%1"'
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\rawspaw"
  DeleteRegKey HKLM SOFTWARE\rawspaw

  Delete $INSTDIR\rawspaw.exe
  Delete $INSTDIR\uninstall.exe

  ; Remove directories used (only deletes empty dirs)
  RMDir "$SMPROGRAMS\rawspaw"
  RMDir "$INSTDIR"

SectionEnd