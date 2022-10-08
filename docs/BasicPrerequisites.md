# Basic Prerequisites for WheelMUD

## Supported Operating Systems
Most recent Windows environments should be able to run WheelMUD. The following have been explicitly reported as working without any code changes:
* Windows 10
* Windows XP (at least with SP2 and SP3)
* Windows Vista (including SP1 and SP2)
* Windows 7
* Windows 2003 Server (including R2)
* Windows 2008 Server (including R2)
Others (such as Windows 8) should work; feel free to prepare a documentation update if you try another operating system.

## Runtime Dependencies
* [.NET Core 6.0+](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). You generally want the "SDK (version number)" section. You do _not_ need the ASP.NET version!
  - Exception: A machine which is only going to _run_ the server and not do development tasks, it only needs the smaller "Runtime" version.

**Troubleshooting**: If you skip the .NET Core step or install the wrong one, RavenDB will crash with console output can feel misleading.
Best to install this explicitly and carefully via the link above, to be certain you have a truly compatible version.
(At this time, .NET Core 6.0.9 is the latest version to be verified working by the maintainer.)
If you're still having trouble, feel free to reach out on the [Discussions area](https://github.com/DavidRieman/WheelMUD/discussions) on GitHub.

## Supported Development Environments
Currently we are focusing on supporting one solution file, for use with [Visual Studio](https://visualstudio.microsoft.com/downloads/) 2019 or 2017.
(The free, full-featured Visual Studio Community 2019 edition, is a good way to go. Paid versions will work as well, if you happen to have them.)

We're not doing anything super fancy with the solution file, so you should even be able to use alternative IDEs for code editing and build the solution with a command-line task, if you want to.
The solution will likely continue working in Visual Studio versions earlier than 2017 as well (although not officially supported).
Solution files existed before for MonoDevelop, SharpDevelop, and a VS2010 Express solution (because that edition did not support solution folders); however, these are not currently supported by the core WheelMUD team.
If you feel blocked because your IDE of choice is not cooperating, feel free to open a thread in the GitHub "discussion" area.
