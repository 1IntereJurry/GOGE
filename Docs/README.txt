GOGE - The Grand Odyssey of Grandiose Explorers

Quick start (development)
- Open the solution in Visual Studio or use the dotnet CLI.
- Build: `dotnet build` (requires .NET 8 SDK).
- Run from IDE or: `dotnet run --project GOGE\GOGE.csproj`.

Publish (create distributable)
- Recommended (Windows x64, self-contained single file):
  dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o ./publish
- The `publish` folder will contain `GOGE.exe` and a `Locales` folder.

Packaging for distribution
- Option A (single-file): include `publish\GOGE.exe` and the `publish\Locales` folder in a ZIP. Recipients can run `GOGE.exe` directly.
- Option B (installer): use the provided Inno Setup script `Installer\GOGEInstaller.iss` (edit paths) and compile it with Inno Setup to create a .msi/.exe installer.

Saves and user data
- Save files are stored per-user in: %LOCALAPPDATA%\GOGE\Saves
- This avoids requiring admin rights when the game is installed under Program Files.

Locales
- The game requires the `Locales` folder (containing `en.json` and `de.json`) next to the executable. Make sure the installer or ZIP includes it.

Troubleshooting
- If the game fails to start on a user's machine and you used framework-dependent publish, ensure .NET 8 runtime is installed.
- If saves cannot be written, verify the application can create the directory `%LOCALAPPDATA%\GOGE\Saves`.

Files added by tooling
- Docs\README.txt (this file)
- Tools\publish.ps1 - convenience script to publish and zip for Windows
- Installer\GOGEInstaller.iss - Inno Setup script template (edit Source paths before compiling)

If you want, I can:
- Adjust the Inno Setup script to embed the save directory creation or to include an option to migrate existing saves.
- Create a GitHub Actions workflow to produce the publish artifact automatically.
