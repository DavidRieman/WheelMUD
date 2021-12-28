# Eventing - Technical Design

## Overview
One of the important techniques WheelMUD uses to promote both extensibility and encapsulation is through the use of C# [eventing pattern](https://docs.microsoft.com/en-us/dotnet/csharp/event-pattern).
Many of the game systems will rely on events to both find out whether an action can be carried out, and then to signal that an action has occurred.

## Eventing Objectives
Through proper eventing, we can reduce the likelihood that a specific game system will need to modify or overwrite entire pieces of Core code. This in turn helps encourage game builders to keep their Core code upgradable from the main repository, and compatible with potential contribution pull requests too, which will help to foster an ongoing community of mutual advancement.

## Requests and Events
Most game actions will be facilitated through one or more Events, and a subclass of Events called "Requests".
We generally refer to these simply as "Requests" and "Events", or to refer to both inclusively we'll refer to "Eventing" instead.

A Request is sent to listeners to find out if any game systems will block (cancel) the action.
Then (if a related Request was not cancelled), an Event is sent to signal to listeners that something has happened.

For example, when a player issues an "east" command to take their character to the next room to the east, a `MovementRequest` will be sent out.
Then, if no game systems Cancel the request, their movement is committed and a `MovementEvent` is sent out.

Systems which want that chance to Cancel a request can subscribe to `OnMovementRequest`, while systems which want to know that such a movement has actually occurred can subscribe to `OnMovementEvent`.

## Encapsulation
This system of Requests and Events helps support encapsulation by separating all the special cases and scenarios.
For example, with Movement, there may be _lots_ of ways which movement can and should fail.
Maybe there's a door in the way that is closed.
Maybe the character is in combat. Or dead, or unconscious. Or currently immobilized by a spell.
Maybe the character is in a mini-game and have to exit that mode first.
Instead of all these, and an infinitely expanding number of _other_ future cases, having to be modifications of the Core movement code: The event system allows movement code to remain focused on movement.
Similarly, the systems which cancel impact movement get to declaratively describe this effect close to where it matters. For example, an ImmobilizationEffect on a player can listen to requests of the player it is attached to, and cancel them as appropriate until the effect has expired.

## Extensibility
In the examples shown above, it is clear to see that _some_ of these systems may be eligible to be game-system-agnostic, Core systems.
Others may depend on specific combat systems or things like "spells" which would only exist in certain systems rather than _most_ games.
While WheelMUD can try to give a lot of "building blocks" that such systems can lean on for reuse (like an ImmobilizationEffect), many games will opt or need to have their own systems to cater to their own unique game play.
It would be a shame if a typical game developer had to frequently modify Core code in a game-specific way, to support their unique edge cases.
It would also be a shame if a developer of a cool piece of interactivity had to build it as a spaghetti mess of modifications to dozens of existing Core code files: This would be very difficult to package up and share with others successfully, which would dampen community sharing and the pace of technological advancement.

## Example: Mini-Game
Let's suppose a developer is building MyMiniGameBehavior, for world builders to attach to `Thing` representing the game board.
This `Thing` would be placed in a room, and will expose a set of "context commands" to characters in the room, including one to join other players on the next round. The behavior sends output to the players frequently each round.

The developer should be able to build this behavior into a small library that can be compiled and "dropped in" to an existing MUD (picked up through MEF extensibility) and immediately available to their world builders to put on an in-game table `Thing`.

Thematically, it doesn't make sense for this approach to allow the players to walk away in the middle of the game, to go off and do combat while still receiving output from the game room that they already left, or walking back in an hour later and instantly being part of the game again, etc. There can be any number of reasons the player suddenly tries to leave (such as being teleported out or having "follow" mode on while their leader comes in and out of the room...) So either MyMiniGameBehavior will want to make you forfeit when you do leave, or block requests to leave with a message like "You have to `forfeit` to leave the game."

The former can be accomplished by subscribing to `OnMovementEvents`. On receipt, if the leaving `Thing` is a player in the game, force them to forfeit.
The latter can be accomplished by subscribing to `OnMovementRequests`. On receipt, if the `Thing` requesting to leave is a player in the game, Cancel the request with the message about the `forfeit` context command.

## Example: Jail
An administrative "jail" command would likely want to teleport a player straight to a special jail room, regardless of whether they were considered immobile from some other system.
So, occasionally, it may make sense for an action to be uncancellable: The system (like the jail command) could simply go straight to the Event instead of first raising a Request.
Note though that this is itself a dangerous thing to do as it can break the assumptions that other systems have made about their state.
For example, if the character was in combat, the combat system might not be designed to handle that sudden disappearance of a combatant gracefully, since it may be itself designed to Cancel any player movement requests. Or if the player was in a mini-game, perhaps they'll still be in it after being jailed, messing up the state of the mini-game for other players.

So, one should carefully consider why it seems important to skip the Request part of the Request+Event pattern before moving forward. In this case, a safe alternative may be to have the jail command Request to teleport the player, but provide any cancellation feedback to the admin so they can manually force the player into the appropriate state (such as ending the combat, or forcing the player to issue a "forfeit" command to exit the mini-game first).

A jail command would also probably place something like a JailBehavior on the player. The JailBehavior could subscribe to OnMovementRequest for the player and cancel ALL of them.
We can see how this would help avoid "jail breaking" opportunities, even against _future_ systems: Imagine a "summon" spell was only recently added to the game: So long as the spell follows the correct Request+Event pattern for moving the target player, the JailBehavior will protect against such a system from being a jail-break opportunity without any direct consideration of this scenario.
The same would be true if the character was carrying a portable portal they tried to "enter" or whatnot, or any other form of edge case movement capabilities.

## Sensory Events
Often, a game system will want to print some detail to observers, but to do so in a way that makes sense for their current sensory state.
Printing something like "you see ..." doesn't make sense if the character is currently blind, and "you hear ..." doesn't make sense if the character is currently deafened.
(TO DO: Write and link to Sensory Systems in more detail!)
To facilitate these easily, we have SensoryEvents which take a SensoryMessage, for propagating those the same way as other events.
Any eligible witnesses to the propagated event will see the version of the output that makes sense for their particular context.

## Event Scope
Eventing methods all take an object that describes the event (and usually contains a `SensoryMessage`), and a value describing the `EventScope`.
The `EventScope` dictates who gets a chance to handle or witness it. For example, a player who issues a "smiles" emote generates a `SensoryMessage` for their Request+Event. The Request will be sent with `EventScope.ParentsDown` relative to the player `Thing` itself, meaning the eligible witnesses may be the room itself and all the players within. After the _Request_ resolves with no cancellations, the same object and scope value will be sent as an _Event_. Receiving that event is when each witness uses the sensory system to pick which message will be printed to them (if any at all) such as "Bob smiles" (or perhaps nothing if the witness is, say, blinded right now).

### Event Scope Propagation
An interesting aspect of the current event scope propagation scheme is that usually the whole child tree of the broadcast point gets to witness events. For example, if a player is holding an intelligent weapon or a scrying object or has a "tiny" character on their shoulder or in their backback, or a player in a "car" sub-Thing of the "room"... All of these sub-Things will get a chance to witness any events (like "Bob smiles").

This makes sense in most cases, but is not optimal in others. For example, the tiny character in another's closed backpack gets to "see" outside the backpack. For now this feels like the right level of correctness versus complexity, as such cases won't be "real" scenarios in a typical MUD and may generally be forgiven by a player of such a complex game anyway. For ideas and discussions where we might want to go on this, check @@@ DISCUSSION THREAD LINK @@@.

### Global Events
There are also global events attached to specific systems. For example, when a player has logged in, a `GlobalPlayerLogInEvent` is sent through the `PlayerManager` to any listeners. To build a "friends" system that notifies a player whenever one of their friends has logged in, one could subscribe to `GlobalPlayerLogInEvent`: The reacting code could check to see if the logged-in player is one of their friends, and if so, notify them that their friend has just logged in. It would silently ignore all other logins.
