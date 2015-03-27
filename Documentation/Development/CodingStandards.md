# WheelMUD Coding Standards

## StyleCop
We are using StyleCop to help us keep a handle on coding standards and consistency throughout the code-base.
We have only turned off a few of the rules, which most egregiously flew in the face of very common modern conventions. A StyleCop rule must be actively fighting our project goals for us to consider disabling it.
The disabled rules are:
* SA1309 "FieldNamesMustNotBeginWithUnderscore" -- The fact is that most of the world has decided to stick with underscore and it has shown to be a big sticking point for people. So we may as well conform to popularity here to be more approachable. (Even though most of the code-base used the "this." pattern instead, which was functionally more useful by applying enforcing syntax rather than naming convention.)
* SA1101 "PrefixLocalCallsWithThis" -- Goes hand in hand with SA1309, in opting for the underscore-only naming convention without syntax enforcement, due to sheer popularity.
* SA1200 "UsingDirectivesMustBePlacedWithinNamespace" -- The rule itself provides dubious benefit, at best. This rule also seems very unpopular, and again seems to be a big sticking point for people, so we've disabled it.
Debates about style could easily go on forever if we let them; We are not likely to adopt many rules changes, but those can be discussed over at the [forums](muds.gamedev.com) as needed.
There are two versions of StyleCop:
* StyleCop for Visual Studio
* StyleCop for Resharper

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
// <summary>
//   A command that lets a character give items to a character or a mob.
//   Created: April 2009 by Karak.
// </summary>
//-----------------------------------------------------------------------------
```
When creating a new file, just copy-paste an existing header and update the file name and the summary.
You can omit the "Created:" style comments if you wish. Conversely, if you wish, you can add your own lines like "Updated: Added the Whatever feature, 2015 by Karak" when making non-trivial updates.
