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
