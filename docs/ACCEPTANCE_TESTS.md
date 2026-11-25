# Physical acceptance checklist
All entries initially NOT RUN. Record date, build SHA, Windows version, device model/Android version, expected/actual result and sanitized evidence. Use owned or explicitly authorized test devices; back up data before write operations.
| ID | Scenario | Expected |
|---|---|---|
| A01 | Windows 10/11 x64 install, launch, uninstall | No elevation by default; shortcuts and clean removal |
| A02 | No device / missing platform tools | Clear error; app remains responsive |
| A03 | Unauthorized then authorized USB ADB | Unauthorized reads blocked; authorized target appears |
| A04 | Two devices and disconnect during read | Explicit target; no unintended device operation |
| A05 | Identity, battery, storage, memory | Values match independent device observations |
| A06 | Fastboot discovery/read-only getvar | Correct target; no writes |
| A07 | Shared backup, verify, staged restore | Original files untouched; independently compare restored bytes |
| A08 | Trusted test APK install/uninstall | Confirmation and correct target; disposable app only |
| A09 | Recovery reboot | Confirmation, battery/backup checks for guided path; observed or unverified result |
| A10 | CVE search and curated bulletin import | No false confirmed vulnerability claims |
| A11 | Local Ollama with reviewed evidence | Advisory response; no automatic device action |
| A12 | Report export and technician history | Correct escaped output; no unintended sensitive data |
| A13 | Firmware mismatch | Flash remains disabled |
| A14 | Concurrent operations | Second participating operation rejected |
| A15 | Keyboard navigation, scaling, module search | Usable UI at tested DPI and keyboard access |
| A16 | Subscription preview | Prices correct; no charges or misleading activation |
