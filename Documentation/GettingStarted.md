# Getting Started with WheelMUD

## Prerequisites
Basically any modern Windows environment with any popular C# development environment, can be used.
However, we currently recommend using Visual Studio 2013.
For more details, check the [Basic Prerequisites](BasicPrerequisites.md) page.

## Additional Requirements
* .NET Framework 4 (http://www.microsoft.com/en-us/download/details.aspx?id=17718).
* Any Telnet client, such as:
 * [zMUD or cMUD](http://www.zuggsoft.com/index.php). Heavy, feature-rich telnet client, used by tons of serious MUD users.
 * [PuTTY](http://www.putty.org). Lightweight telnet client, but remembers session info.
 * [fTelnet / HtmlTerm](https://www.ftelnet.ca). Lightweight Flash and HTML5 based clients.
 * Telnet.exe: Built into all supported versions of Windows, but the Telnet Client feature generally starts disabled. Open up the "Turn Windows features on and off" control panel, scroll down and ensure "Telnet Client" gets selected.

## Getting the Source Code
It is preferred to create a GitHub fork the code from the [WheelMud/WheelMUD](https://github.com/WheelMud/WheelMUD) repository.
On the right side of GitHub you will see the "HTTPS clone URL" which you can use with git to pull down a local repository.
If you don't know how to work with git and GitHub, you will want to follow a few tutorials on those tools first.

## Building and Launching the Server
We take pride in keeping the initial set-up process as painless as possible.
* Open up the appropriate .sln file; generally WheelMUD.sln.
* Ensure TestHarness is the StartUp project.
* Build and run.
That's it! For example, if your development environment of choice is Visual C# 2010 Express:
* Open up the WheelMUD_Express.sln solution file.
* In Solution Explorer, collapse projects expanders, right-click TestHarness, "Set as StartUp Project".
* Ctrl+F5.
The first time the server application is running, you may be prompted with firewall adjustments; accept these for all networks.

## Connecting Telnet
You can now open up your telnet client of choice and connect to localhost 4000.
For example, if you just have the standard Telnet.exe, Run (Windows+R) Telnet.exe, then type "open localhost 4000" at the "Microsoft Telnet" prompt.
You should be greeted with a WheelMUD welcome screen, and the TestHarness console will print that a connection has been accepted.

## Optional Tools
* [TortoiseGIT](https://code.google.com/p/tortoisegit) can help a lot, if you are not interested in being a git command-line guru.
* [GitHub for Windows](https://windows.github.com).
* [RavenDB](http://ravendb.net) is used for most world/character data.
* NuGet can be used to manage dependency versions as found in src/packages. (We do commit those packages to ensure they will be present even if their on-line presences are down, as has occurred sometimes in the past.)
