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
* [.NET Core release 5.0.0](https://dotnet.microsoft.com/download/dotnet/5.0#5.0.0): Either the "SDK 5.0.100" (_not_ the "SDK 5.0.101" version) or the "ASP.NET Core Runtime 5.0.0" (_not_ the ASP.NET Core Runtime 5.0.1).
If you skip this or install the wrong one, RavenDB will crash in a way that we cannot crash, and present console output which may be misleading. Best to install this explicitly and carefully via the link here, to be certain you have a truly compatible version.
* .NET Core 3.1 as well. WheelMUD code itself targets this framework. Visual Studio itself should successfully ensure it is present, in this case.

## Supported Development Environments
Currently we are focusing on supporting one solution file, for use with [Visual Studio](https://visualstudio.microsoft.com/downloads/) 2019 or 2017.
(The free, full-featured Visual Studio Community 2019 edition, is a good way to go. Paid versions will work as well, if you happen to have them.)

The solution will likely continue working in Visual Studio versions before 2017 as well (although not officially supported).
Solution files existed before for MonoDevelop, SharpDevelop, and a VS2010 Express solution (because that edition did not support solution folders); however, these are not currently supported by the core WheelMUD team. If you are blocked because your IDE of choice cannot open the current main WheelMUD.sln, please open a thread in the "discussion" section of GitHub.
