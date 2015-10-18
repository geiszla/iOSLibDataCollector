# iOSLibDataCollector
This is a sample project in C# for libimobiledevice library. It makes a directory tree from (the accessible) /var/mobile/Media folder, locates the Photos.sqlite database and copies it to the computer. Although it's a pretty specific task, it's good for understanding the usage of some sections (AFC, Lockdown, etc.) of libimobiledevice library in C#. It also logs the process in order to find errors easily.

##Releases
Releases can be found under the "Releases" tab or by clicking [here](https://github.com/geiszla/iOSLibDataCollector/releases).

(Note: You need the dlls to be in the same folder as the ".exe" to make the program work.)

##Requirements
Requires .NET 4 (Client profile).
<br />Tested on iOS 4.1.2 -> 8.4, but probably works on lower versions, and as long as higher versions is supported by libimobildevice, they will be supported too. <b>Doesn't require jailbreak.</b>

##Setting up
Due to the fact, that LibiMobileDevice library is only compiled to 32-bit (x86) Windows, you have to <b>change the target CPU architecture of the project</b>. To do it click "Any CPU" at the toolbar (next to "Debug") and select "x86". If it isn't already there, click "Configuration Manager...", then click "Any CPU" at the top of the newly opened window, select "<New>", select "x86" instead of "x64", click OK, click close and you're done. Note, that you have to change this again if you change "Debug" to "Release".

##Change log
<h4>1.0</h4>
Initial release
<h4>1.1</h4>
 - Added logging for better error detection
 - Small bug fixes

<h4>1.2</h4>
 - Fixed an issue which prevented data collection on iOS 5 and older devices.
 - More small bug fixes.
