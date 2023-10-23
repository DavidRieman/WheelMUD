# WheelMUD Coding Standards

## StyleCop
NOTE: This section became outdated.
We are likely to be [changing our official style validation](https://github.com/DavidRieman/WheelMUD/issues/24) tactics soon.

We used to rely on StyleCop for automatic code styling/standards/consistency validation throughout the code-base.
We had only turned off a few of the rules, after discussions. Rule disables must be actively fighting our project goals for us to consider disabling it.

## Clean Up for Pull Requests
Please follow these guidelines before making a pull request.
* Have the code be under test. (We are really trying to adopt TDD but have historically been far from that.)
* Tell Visual Studio to auto-format the documents you touched, with the default formatting settings. (This will also reduce StyleCop violations.)
* Run StyleCop and clean up violations in the code.
While we might deny pull requests which go out of their way to restyle existing things against established StyleCop rules, we probably won't deny pull requests just because new code didn't strictly adhere to every rule.
Basically, please try to follow the styling examples set throughout the code-base, and run StyleCop if you want to be a great citizen.
If this is too much burden for your volunteer time, don't worry too much about it; The core team will eventually take care of those final bits of full compliance on a future compliance pass.

## File Headers
The file headers will look like this:
```
//-----------------------------------------------------------------------------
// <copyright file="Give.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------
```
When creating a new file, just copy-paste an existing header and update the file name.
Do not include any 'created', 'edited' or 'updated' elements in the header. Git is used as the history tracking solution for all files in this project. Please add comments to your commits explaining any changes made to the code.

## Namespaces
We are trying to use the [.NET standard "Names of Namespaces"](http://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces) guidance, going forward.
(We definitely had too many and are dialing this back; Names that ended like ".Session" and ".Enums" introduced various problems.)
