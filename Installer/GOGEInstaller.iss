[Setup]
AppName=GOGE
AppVersion=1.0
DefaultDirName={autopf}\GOGE
DefaultGroupName=GOGE
OutputBaseFilename=GOGE_Installer
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Edit the Source paths to the published files on your machine before compiling
Source: "{#SourcePath}\GOGE.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourcePath}\Locales\*"; DestDir: "{app}\Locales"; Flags: recursesubdirs createallsubdirs
Source: "README.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\GOGE"; Filename: "{app}\GOGE.exe"
Name: "{userdesktop}\GOGE"; Filename: "{app}\GOGE.exe"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked

[Run]
Filename: "{app}\GOGE.exe"; Description: "Start GOGE"; Flags: nowait postinstall skipifsilent
