# Basic Prerequisites for WheelMUD

## Supported Operating Systems
Most recent Windows environments should be able to run WheelMUD. The following have been explicitly reported as working without any code changes:
* Windows XP (at least with SP2 and SP3)
* Windows Vista (including SP1 and SP2)
* Windows 7
* Windows 2003 Server (including R2)
* Windows 2008 Server (including R2)
Others (such as Windows 8) should work; let us know how it goes in the [muds.gamedev.com](http://muds.gamedev.com) forums if you try another operating system.

## Supported Development Environments
Currently we are focusing on supporting one solution file, for use with [Visual Studio](https://visualstudio.microsoft.com/downloads/) 2019 or 2017.
(The free, full-featured Visual Studio Community 2019 edition, is a good way to go. Paid versions will work as well, if you happen to have them.)

The solution will likely continue working in Visual Studio versions before 2017 as well (although not officially supported).
Solution files existed before for MonoDevelop, SharpDevelop, and a VS2010 Express solution (because that edition did not support solution folders); however, these are not currently supported by the core WheelMUD team. If you are blocked because your IDE of choice cannot open the current main WheelMUD.sln, please open a thread in the "issues" section of GitHub.
