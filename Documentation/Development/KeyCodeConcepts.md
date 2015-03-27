# WheelMUD Key Code Concepts

## Thing
This is the basic building block for any interactive object object in the game world; whether it be a player, an enemy, a room, a door, a weapon, or even an area itself.
Things can have a collection of child objects; a player Thing may have children representing the player's equipment and inventory.
(Note that we capitalize Thing when communicating about this code class, to avoid confusion.)
Things can have any number of children, who can in turn have children, and so on.
For example, an area may have many rooms as children, and one of those rooms may have items and players and exits as children, the player may have equipment and inventory items as children, and so on.
The child graph should always form a Directed Acyclic Graph (DAG); In other words, a Thing should never appear in its own children tree.

## Behavior
Any Thing can be adorned with Behaviors. Behaviors modify how a Thing works, giving or taking away capabilities, interactivity, etc.
In other frameworks, one might extend the capabilities of objects through inheritance (or worse, hacking a multitude of systems to support some edge cases for new behaviors).
However, here, Behaviors extend Things in a way that is highly encapsulated and highly dynamic.
For example, we can write an OpensClosesBehaviors once and apply it to a Thing that we intend to be a door.
We could apply it to a Thing that we intend to be a treasure chest.
It does not matter that we wrote it without considering the case of vehicles; we can apply it to a Thing we intend to be a car, and without any code changes, our car now has a functional, interactive door.
A world builder can compose highly interesting objects (without writing any code) by combining and configuring these behaviors.

## Effects
Effects are basically Behaviors which also have a concept of duration. One might build ongoing spell effects, poisons, or other temporary states such as being "stunned" as Effects.

## Actions
Actions are the commands that a player can issue.
They inherit from GameAction and are adorned with a number of attributes that help expose the command to the system, and to the user through the "help" Action and the "commands" Action.
Note that lots of "interactive" actions the user can perform are actually dictated through relevant Behaviors instead of global Action commands.

For example, there is no global "west" nor "move" command.
Instead, the command is presented in a context-sensitive way by the presence of an ExitBehavior.

## Database
We are in the process of moving away from a relational database model, to a model that is largely (or completely) driven by a document database. Eventually, this will make it very easy to import/export/share arbitrary aspects of the game world, such as entire areas, or even just single interesting objects.

## Telnet
Text-based MUDs are generally built on the Telnet protocol.
This is a core aspect of WheelMUD.
There are a lot of useful telnet options which will be supported out of the box, which upgrade the player's experience automatically depending on the supported feature set of their Telnet Client.
There are many Telnet clients out there; many established MUD players utilize powerful clients like zMUD/cMUD, but even Windows has the "Telnet.exe" client (which can be enabled out of the box through the Programs and Features / "Turn Windows features on and off" control panel).

## Rule Sets
A rule set is what defines a particular gaming system.
Server administrators should have the option of starting with a particular rule set, or building their own from scratch.
The core WheelMUD team has selected "[Warrior, Rogue & Mage](http://www.stargazergames.eu/games/warrior-rogue-mage)" as the sample reference rule set, due to its simplicity and open nature.
Additional rule sets may show up in the roadmap after version 1.0.

## DLL Extensibility
The WheelMUD server discovers components of the game through MEF / reflection.
One could drop in the DLL for a given ruleset and remove another, and relaunch the game to get a whole new game experience.
Ideally, some day we'd like to support the idea of "non-reboot upgrades" where a new DLL can be dropped in to the production server, and the server discovers it and starts using the new versions of the classes for all new interactions, without even having to reboot / kick off all the players.
While this isn't a reality yet, it does demonstrate how our server is powerfully composable even at the DLL level.
