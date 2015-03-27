# WheelMUD Features

## Implemented Features
WheelMUD version 0.4 includes:
* Telnet server with command buffering.
* Auto-negotiated ANSI support. 
* Auto-negotiated MCCP support (MUD Client Compression Protocol). 
* Auto-negotiated MXP support (MUD eXtension Protocol). 
* Character creation system.
* Behaviors system. (Object extensibility though composition of highly self-sufficient Behaviors and Effects.)
* Full persistence between resets.
* Self-sufficient persistence tree. (IE player save, makes inventory save, makes items save, makes behaviors save...)
* Movement, including rooms, exits, and doors.
* Basic items and inventory management.
* Basic moving AI entities. 
* Sensory system. (Handling for character blindness, deafness, and so on.) 
* Extensible command and scripting system. 
* Eventing and reaction system. (Events can be propagated throughout the world, and everything has an opportunity to react to events.) 
* Container support. 
* Off-line Editor (may need significant rework to accommodate new document DB model).
* Some skills support.
* Plug-and-play rule sets support, with a basic "Warrior, Rogue, and Mage" rule set started.
* Self-sufficient service for deployments. (If the game crashes, the service restarts.)
* Easy set up and one-key debugging. (F5 to launch TestHarness. No arcane manual steps required.)
* Significant StyleCop code compliance for consistent code.
For more information on topics such as our Project Goals and Getting Started, check out the [TODO Documentation].

## In the Pipeline
WheelMUD version 0.5 will include:
* Basic reference rule set implementation (Warrior Rogue and Mage). 
* Simple Combat. 
* Reference Skills and Spells. 
* Reasonable mob AI. 
For a long term view, you can review our High Level Road Map from the [TODO Documentation].
