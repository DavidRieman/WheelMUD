# Configuring WheelMUD

## Server Data
When building and running the server for the first time, a bunch of files are copied over to the server's "program data" location where it can persist and evolve (such as when new characters are saved to the database).
This program data location depends on your version of Windows. For example, by default:

| OS                     | Folder                                                                     |
| ---------------------- | -------------------------------------------------------------------------- |
| Windows XP Pro SP3     | C:\Documents and Settings\All Users\Application Data\WheelMUD\WheelMUD.net |
| Windows 2003 R2        | C:\Documents and Settings\All Users\Application Data\WheelMUD\WheelMUD.net |
| Windows 2008 R2        | C:\ProgramData\WheelMUD\WheelMUD.net                                       |
| Windows Vista          | C:\ProgramData\WheelMUD\WheelMUD.net                                       |
| Windows 7 (x86 or x64) | C:\ProgramData\WheelMUD\WheelMUD.net                                       |

## Updating Default State
The Server Data files were copied from the SVN folder at systemdata\files.
So if you have changes to the default data state that you wish to check into source control for future servers, you would want to edit the instances at systemdata\files and also update your own Server Data copies for testing.

## Config Files
* MUD.config: Some basic MUD configuration can be adjusted in the TestHarness MUD.config file. This determines things like which port to open, which rule set to use, which files to use as view templates, etc.
* App.config: Application-level configuration, such as logging configuration, are found in the TestHarness App.config file.
* connectionstrings.config: Used for SQL connection information; lives in the Server Data area. (TODO: revisit this.)
* Another copy of MUD.config and App.config are found in WheelMUD.WindowsService. (TODO: make one set use file links, or at least split configs where common to reduce redundancies?)

## Installing as a Service
WheelMUD can be configured to run as a Windows service. The base WheelMUD.exe is used not only for running the server in console mode, but can be used to install the service, run the service, start and stop the service, etc.
We utilize [Topshelf](https://github.com/Topshelf/Topshelf) to accomplish all this with just the one convenient executable.
To get started:
* Ensure you have a copy of the WheelMUD bin folder somewhere you want to run the service from.
* (optional) You may wish to change the port number from 4000 in the "mud.config" file if you plan to host a server permanently, so you can still use the default port for debugging an additional server instance in your IDE as necessary without having to stop the hosted server.
* Open your command prompt or other shell as an administrator, since working with Windows services generally requires administrator privileges.
* Navigate to your bin folder.
* Run "WheelMUD.exe help" to see your options.
* Run "WheelMUD.exe install" and run "services.msc" if you want to see it in the Windows Services control panel. (it will be configured by default in services.msc for automatic startup, for whenever you reboot your computer.)
* Run "WheelMUD.exe start" to start it up from command-line if you want, or you can use services.msc to start it.
* After a few seconds, run "telnet.exe localhost (port number)" to ensure it has started and is working correctly.
