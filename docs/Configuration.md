# Configuring WheelMUD

## Server Data
When running the server for the first time, a bunch of files are copied over to the "ApplicationData" location where it can persist and evolve (such as when new characters are saved to the database).
This program data location depends on your OS. For example, on Windows you may find it at:
`C:\Users\[YourUserName]\AppData\Roaming\WheelMUD\WheelMUD`
From there, that instance data can evolve as players are created, commands are executed, and so on.

If you want to perform periodic backups of all the evolved game data of an active MUD, an easy way to do so might be to copy the whole directory to a safe backup location.

If you wish to later migrate WheelMUD to running in another user context (such as via a Service with a dedicated user account), you may want to manually migrate this data to the ApplicationData area of that new context.

## Updating Default State
The initial Server Data files were copied from the systemdata\Files folder of the WheelMUD Git repository.
So if you have changes to the default data state that you wish to check _back_ into source control for future servers to start with as their starting world data: You would want to edit the instances at systemdata\Files as well as updating your own Server Data copies for testing.

## Config Files
All high level application configuration (from database selections through gameplay options) are now performed via the ServerHarness `App.config`.
Although the default options should be sufficient for launching and checking out WheelMUD at runtime, you can read through [the comments there](https://github.com/DavidRieman/WheelMUD/blob/master/src/ServerHarness/App.config) to understand your configuration options.

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
