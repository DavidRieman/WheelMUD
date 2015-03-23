# How To: Build Sensory Messages in WheelMUD
The SensoryMessage system is key to rendering immersive game output strings to the players whose characters may be under varying effects, such as partial blindness or deafness, or whose character races may have extended senses such as heat vision, night vision, etc.
An important and powerful aspect of SensoryMessages is that they can be broadcasted and ultimately evaluate appropriately for each recipient, without the source Actions having to know anything about all the recipients' current situations.

## Type and Strength
SensoryMessages have one or more SensoryTypes such as 'Hearing' or 'Sight' which signify which senses can be used to perceive the message. It also has a Strength, which signifies how strong the input is. 100 is normally used as the value for a full-effect. A bright light would be 100, whereas a small candle might be a 20. If two people are in the same room, then a 'say' command should have a 'hearing' strength which is less than that of a 'yell' command from the same room.

## ContextualStrings
SensoryMessage works with any ContextualStringBuilder. It is usually given a ContextualString to deliver the message, which is a simple wrapper for when you want to just customize base strings based on receiver/originator/others.

## Example
This example is from the Yell action, from WheelMUD v0.4:
```
  var contextMessage = new ContextualString(entity, null)
  {
      ToOriginator = "You yell: " + this.yellSentence,
      ToReceiver = "You hear $ActiveThing.Name yell: " + this.yellSentence,
      ToOthers = "You hear $ActiveThing.Name yell: " + this.yellSentence,
  };
  var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);
  this.yellEvent = new VerbalCommunicationEvent(entity, sm, VerbalCommunicationType.Yell);
```
As that event gets broadcasted:
* If you were deaf, you'd see nothing at all from it.
* Else if you sent this message, you'd see: "You yell: This is an example!"
* Else if you heard this message from Shabubu, you'd see: "You hear Shabubu yell: This is an example!"
* One might omit the ToReceiver in such an example, but if the room itself has the ability to hear, that line defines what it would hear.

## Future Potential
Messages which traverse multiple rooms, like yells, could spread outward one room-radius at a time and reduce their message strength for each room they are broadcasted away from the origin. Then 'say' commands could do the same thing, going out one room but being at a very low volume, and we could add a 'whisper' command that definitely doesn't leave the room. Closed exits could either impede messages fully or just impact their strength dramatically. (Ideally we should centralize the 'spread out across rooms' iterative functionality with a method that takes an origin and a function.)

We may also want to come up with a mechanism for handling more complex combinations of senses and fall-backs so that users generally see one thing, but if they have to rely on a secondary sense, they might get another message, or if they barely perceive it at all still, they may get yet a different message, etc. On the other hand, we don't want world builders to be swamped with trying to pander to every possible combination of senses for every little thing, so we'll have to work out a good balance and set a reasonable precedent.
