# Getting Started with WheelMUD

## Prerequisites
Satisfy the [Basic Prerequisites](BasicPrerequisites.md) if you haven't already. (Covers OS versions, runtime dependencies, etc.)

## Additional Requirements
* Ensure you have a Telnet client available, such as:
  - Telnet.exe: Built into all supported versions of Windows, but the Telnet Client feature generally starts disabled. Open up the "Turn Windows features on and off" control panel, scroll down and ensure "Telnet Client" gets selected. To see if you have one ready, open up a Command Prompt and run "telnet localhost 4000", which should print that it's connecting to localhost, but then times out (assuming your WheelMUD server isn't running yet).
  - [zMUD or cMUD](http://www.zuggsoft.com/index.php). Heavy, feature-rich telnet client, used by tons of serious MUD users.
  - [Mudlet](https://www.mudlet.org/) Full featured like zMUD, but free and open source ([code](https://github.com/Mudlet/Mudlet)).
  - [MUSHClient](http://www.gammon.com.au/mushclient/mushclient.htm) Another free client, simple to start but lots of powerful features.
  - [PuTTY](http://www.putty.org). Lightweight telnet client, but remembers session info.
  - [fTelnet / HtmlTerm](https://www.ftelnet.ca). Lightweight Flash and HTML5 based clients.

## Getting the Source Code
It is preferred to create a GitHub fork the code from the [WheelMud/WheelMUD](https://github.com/WheelMud/WheelMUD) repository.
On the right side of GitHub you will see the "HTTPS clone URL" which you can use with git to pull down a local repository.
If you don't know how to work with git and GitHub, you will want to follow a few tutorials on those tools first.

## Building and Launching the Server
We take pride in keeping the initial set-up process as painless as possible.
* Open up the src/WheelMUD.sln file.
* Ensure TestHarness is the StartUp project.
* Build the entire solution (not just the TestHarness dependencies).
* Run the TestHarness.
* The first time the server application is running, you may be prompted with firewall adjustments; accept these for all networks.

That's it! You should see a console output window listing sub-services that spin up with WheelMUD.
Once it says "All services are started. Server is fully operational." then it is ready for players to connect.

## Connecting Telnet
You can now open up your telnet client of choice and connect to localhost 4000.
For example, if you just have the standard Telnet.exe, Run (Windows+R) Telnet.exe, then type "open localhost 4000" at the "Microsoft Telnet" prompt.
You should be greeted with a WheelMUD welcome screen, and the TestHarness console will print that a connection has been accepted.

## Optional Tools
* [TortoiseGIT](https://code.google.com/p/tortoisegit) can help a lot, if you are not interested in being a git command-line guru.
* [GitHub for Windows](https://windows.github.com).
* SQLite is used for the base player documents (including player names and such). [DB Browser (SQLite)](https://sqlitebrowser.org/) can be handy tool to browse this data.
* [RavenDB](http://ravendb.net) is used for most world/character data.
* NuGet can be used to manage dependency versions as found in src/packages. (We do commit those packages to ensure they will be present even if their on-line presences are down, as has occurred sometimes in the past.)

## Optional Configuration
TODO: Describe additional TestHarness-as-service instructions and additional app configuration options like switching DAL repositories.
