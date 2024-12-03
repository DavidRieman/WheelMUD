WheelMUD.Telnet
===============
WheelMUD.Telnet is a highly reusable Telnet server implementation.
It is meant to abstract away the fiddly parts of Telnet protocol and TCP socket handling by providing sensible events to work with.
Although the primary consumer of the library is WheelMUD, it is meant to be shared and reused among other Telnet projects as well.
It may be especially well-suited to other MUD implementations as well.
Many Telnet options negotiators are supplied out of the box, but can also be easily extended with new and custom negotiators.

TODO: UPDATE AS THIS SHAPES UP.

TODO:
* Provide a simple fast way to query whether a given Telnet option is enabled for a given TelnetConnection.
* When we raise the NEW CONNECT EVENT NAME you simply need to tell it what to do with incoming data for that connection. @@@
* BUILD IN a negotiation idle event that signals the first time (only) that there have been no negotiations for it for a while. @@@
* 