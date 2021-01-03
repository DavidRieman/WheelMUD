# WheelMUD Solution Structure

We have tried to organized the code into logical groupings, in solution folders and projects. The current solution structure looks like this:

![SolutionExplorer](img/WMSolutionExplorer.gif) (TODO: Update image!)

Here's a brief explanation of each solution folder: 

| Folder          | Description |
| --------------- | ----------- |
| Data            | The low level database code goes here. Known commonly as the Data Abstraction Layer (DAL). |
| Files           | These are files such as configuration, opening screens, and NVelocity templates. |
| Gaming          | This is where the gaming system implementation goes. |
| MudFramework    | This is where the bulk of the MUD server code lives. |
| Solution Items  | This is where solution-wide shared assembly info lives. |
| Support Servers | This is where servers like FTP are located. |
| Tests           | This is where the unit tests live. We need help here. |
| ServerHarness   | This is a console project which runs the server. |
