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
* [.NET Core 2.2.8](https://github.com/dotnet/core/blob/master/release-notes/2.2/2.2.8/2.2.8.md#downloads) runtime (or SDK):
Out of the box, we default to using RavenDB (embedded mode) for storing world data, which requires this runtime to be present. (You can pick the x64 installer for x64 machines, even for running x86 builds of WheelMUD.)  Please note: Trying to run the app without having this dependency in place may crash RavenDB when you first try to create or load a character, with output in the console output that may be misleading or missing a download link for the actual dependency. Best to install this explicitly via the link here, to be certain you have it.
* .NET Core 3.1 as well. WheelMUD code itself targets this framework. Visual Studio itself should successfully ensure it is present, in this case.

## Supported Development Environments
Currently we are focusing on supporting one solution file, for use with [Visual Studio](https://visualstudio.microsoft.com/downloads/) 2019 or 2017.
(The free, full-featured Visual Studio Community 2019 edition, is a good way to go. Paid versions will work as well, if you happen to have them.)

The solution will likely continue working in Visual Studio versions before 2017 as well (although not officially supported).
Solution files existed before for MonoDevelop, SharpDevelop, and a VS2010 Express solution (because that edition did not support solution folders); however, these are not currently supported by the core WheelMUD team. If you are blocked because your IDE of choice cannot open the current main WheelMUD.sln, please open a thread in the "discussion" section of GitHub.
