**Created by:** Paul Lewis

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/Ixe1/OMSI-Time-Sync-Plugin)](https://github.com/Ixe1/OMSI-Time-Sync-Plugin/releases)

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/ixe1)

[![GitHub issues](https://img.shields.io/github/issues/Ixe1/OMSI-Time-Sync-Plugin)](https://github.com/Ixe1/OMSI-Time-Sync-Plugin/issues) [![GitHub forks](https://img.shields.io/github/forks/Ixe1/OMSI-Time-Sync-Plugin)](https://github.com/Ixe1/OMSI-Time-Sync-Plugin/network) [![GitHub license](https://img.shields.io/github/license/Ixe1/OMSI-Time-Sync-Plugin)](https://github.com/Ixe1/OMSI-Time-Sync-Plugin)

# OMSI Time Sync Plugin
A simple tool which automatically keeps OMSI 2's in-game time in sync with either the system time or Bus Company Simulator virtual company's time. Optionally you can manually sync the in-game time by pressing the button on the UI.

By using this tool it's no longer necessary to manually adjust the OMSI in-game time, typically after every tour or so, due to the game usually lagging periodically and therefore the in-game time drifts from either the system's actual time or virtual bus company's actual time.

# Screenshot (v1.10)
![OMSI Time Sync](https://user-images.githubusercontent.com/96985590/149658307-5dea03b8-3cee-44ce-b7eb-71d27597fd39.PNG)

# Video Preview (v1.00)
https://user-images.githubusercontent.com/96985590/147923517-8446a49a-b5c6-478c-9d2f-65b2b7f9d84c.mp4

**Important:** This tool modifies parts of the memory of OMSI.exe. It's strongly recommended that you close any games which have anti-cheat detection prior to running this tool as this activity might be falsely flagged as a cheat/hack by one or more of the various anti-cheat solutions out there.

# Prerequisites
- OMSI 2 **version 2.2.032 or 2.3.004** - this tool will not work with other versions of OMSI 2
- .NET Framework 4.7.2 Runtime (https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)

# Installation Steps
1. Download the latest release from [here](https://github.com/Ixe1/OMSI-Time-Sync-Plugin/releases)
2. Extract the ZIP file to somewhere convenient
3. Copy the OmsiTimeSyncPlugin.dll and OmsiTimeSyncPlugin.opl files to your OMSI2's plugins directory
4. Run OMSI2, ideally without Bus Company Simulator initially
5. Acknowledge initial notice and configure OMSI Time Sync as appropriate
6. Play OMSI or Bus Company Simulator and continue as usual

# Features
- The MPH/KMH shown on the title bar can be toggled, double click the text to switch between them
- The UI can be made to be 'always on top' or not by clicking the first button on the UI (disabled is red, enabled is green)
- The UI can be expanded or compacted with the second button on the title bar
- The UI can be minimised by clicking on the third button on the UI
- The UI can be re-positioned by left clicking and then holding on to the title bar and moving the mouse

**Note:** If you're in a virtual company in Bus Company Simulator then please make sure you set the correct 'OMSI Time Offset' for that virtual company's timezone so that the OMSI in-game time is correct when it's being synced.

# Code Signed Digital Certificate
Any compiled DLL file release officially done by Ixel / Paul Lewis, the author of OMSI Time Sync, will have the following serial number:
- 64e5cc4297c711bc4e113d40fb67a459

If this serial number doesn't match or the digital certificate is missing on the DLL file then it has likely not been compiled by the author of OMSI Time Sync.

# Questions?
Contact me on Discord at Ixel#6107 or send something via Github.

# Donation
While this is a free and open source program, if you like this tool then a donation of some kind is highly appreciated. Doing so also encourages me to develop something else in the future that you and others might find useful, as well as to maintain this specific tool as and when it might be necessary or appropriate.

Thanks in advance to anyone who chooses to donate something.

**Donate at https://paypal.me/ixe1**

# Licence
This is licenced under the GNU General Public License v3 (GPL-3) licence. Please see https://www.gnu.org/licenses/gpl-3.0.en.html for more information.

# Credits
- Ixel - Main author who created this program - Contact either on Discord at Ixel#6107, Fellowsfilm forum at https://fellowsfilm.com/members/ixel.46491/, OMSI WebDisk forum at https://reboot.omsi-webdisk.de/community/user/15732-ixel/ or alternatively Github at https://github.com/Ixe1
- [sjain](https://github.com/sjain882) - Helping beta test both the tool and the plugin, idea about anti-cheat warning notice, concept for AoB scanning game version
- [BlueOrange](https://github.com/BlueOrange775) - C#.NET OMSI plugin base template
- Adam Mathieson (GPM Technical Systems) - Assistance with some memory addresses and pointers for getting certain details from OMSI2's memory
- Charlie S#6270 on Discord - Helping beta test both the tool and the plugin
