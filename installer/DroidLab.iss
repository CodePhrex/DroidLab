#define AppVersion "1.0.0-rc.1"
[Setup]
AppId={{D714B990-2A91-4E1B-97A8-1B4B991F0C22}
AppName=Droid Lab
AppVersion={#AppVersion}
AppPublisher=CodePhrex
AppPublisherURL=https://github.com/CodePhrex/DroidLab
DefaultDirName={localappdata}\Programs\CodePhrex\DroidLab
DefaultGroupName=Droid Lab
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=..\dist
OutputBaseFilename=DroidLab-Setup-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\DroidLab.Desktop.exe
CloseApplications=yes
[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; Flags: unchecked
[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
[Icons]
Name: "{autoprograms}\Droid Lab"; Filename: "{app}\DroidLab.Desktop.exe"
Name: "{autodesktop}\Droid Lab"; Filename: "{app}\DroidLab.Desktop.exe"; Tasks: desktopicon
[Run]
Filename: "{app}\DroidLab.Desktop.exe"; Description: "Launch Droid Lab"; Flags: nowait postinstall skipifsilent
