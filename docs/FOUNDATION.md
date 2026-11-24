# Foundation v0.1.0
Droid Lab by CodePhrex — Beyond Device Management.
Approved direction: Windows, C#, .NET 10, WPF, modular MVVM, SQLite, ADB/Fastboot and OEM adapters, optional local/cloud AI, CVE advisory correlation, authorized servicing and reporting.
Palette: #0B1119, #182433, #00E5B0, #398CFF, #F2F6FC. Orbitron branding and Inter interface are targets.
This initial shell exposes 16 navigation destinations; only read-only ADB discovery is implemented. No flashing, recovery, backup, security scanning or SIM operations are implemented yet. Physical validation is pending. The shell currently uses code-behind; extract transport and view models before expanding functionality.
Build: dotnet build src/DroidLab.Desktop -c Release on Windows with .NET 10 SDK.
ADB requires official platform-tools on PATH.
Never bypass device authorization or perform destructive operations without explicit confirmation.
CI must pass before merging; configure required windows-build status check on main.
