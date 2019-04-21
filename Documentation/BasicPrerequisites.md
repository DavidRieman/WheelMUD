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
* [.NET Core 2.1.10](https://github.com/dotnet/core/blob/master/release-notes/2.1/2.1.10/2.1.10.md#downloads) runtime (or SDK):
Out of the box, we default to using RavenDB (embedded mode) for storing world data, which requires this runtime to be present. (You can pick the x64 installer for x64 machines, even for running x86 builds of WheelMUD.)
* [.NET Framework 4.7.2](https://support.microsoft.com/en-us/help/4054530/microsoft-net-framework-4-7-2-offline-installer-for-windows):
WheelMUD code targets this framework. However, the targeted frameworks can all be lowered to around 4 or so if you need to, without very much hassle.

## Supported Development Environments
Currently we are focusing on supporting one solution file, for use with [Visual Studio](https://visualstudio.microsoft.com/downloads/) 2019 or 2017.
(The free, full-featured Visual Studio Community 2019 edition, is a good way to go. Paid versions will work as well, if you happen to have them.)

The solution will likely continue working in Visual Studio versions before 2017 as well (although not officially supported).
Solution files existed before for MonoDevelop, SharpDevelop, and a VS2010 Express solution (because that edition did not support solution folders); however, these are not currently supported by the core WheelMUD team. If you are blocked because your IDE of choice cannot open the current main WheelMUD.sln, please open a thread in the "issues" section of GitHub.
