# WheelMUD Road Map
This is a high level road map. For a list of specific completed and upcoming features, see [Features](Features.md).

## Features By Version
Contributors are welcome to work on features that are not in the next upcoming version.
However, focusing on the nearest upcoming features is a way to help ensure we're all ready to think about those features, and reduces the chance of needing to significant rework.
The GitHub Issues can be filtered by "milestone" which map to our planned "release version" here.
Our road map may change over time. Most likely a few features that are less common in MUDs will be punted to the 1.1+ version as we approach and re-evaluate them.

| Version | Status      | Description |
| ------- | ----------- | ----------- |
| 0.1     | Done        | (History lost; built before GitHub and our prior hosting.) |
| 0.2     | Done        | (History lost; built before GitHub and our prior hosting.) |
| 0.3     | Done        | Was basic persistence (rooms, items, mobs), logging. |
| 0.4     | Done        | Was plug-in systems, basic remote administration, Windows service, equipment, race, attributes, alignment. |
| 0.4.1   | In Progress | [Improve persistence](https://github.com/DavidRieman/WheelMUD/projects/1). [Equipment](https://github.com/DavidRieman/WheelMUD/projects/2), [race, attributes, alignment](https://github.com/DavidRieman/WheelMUD/projects/3) need to be rebuilt. |
| 0.5     | Planning    | [Basic WR&M combat](https://github.com/DavidRieman/WheelMUD/projects/4), [mob AI](https://github.com/DavidRieman/WheelMUD/projects/5), [skills, spells](https://github.com/DavidRieman/WheelMUD/projects/6), [finish Behaviors refactoring and related tests](https://github.com/DavidRieman/WheelMUD/projects/7). |
| 0.5.1   |             | Playability polish. (Have a "working game" reference implementation with some sort of character advancement, and related polish). |
| 0.6     |             | Party, guilds, chat channels, settings, economy, weather. |
| 0.6.1   |             | Documentation polish. (Try to help early adopting game admins to get up and running on our important dev model/concepts and administration tactics).
| 0.7     |             | Quests, paging, message boards, ticketing. |
| 0.8     |             | InterMUD communication, player housing / in-game building (OLC) (though OLC likely needs to move up in priority), professions. |
| 0.9     |             | Admin-events support, pets (and mounts), MCCP. |
| 1.0     |             | Performance and stress testing, optimizations, release polish, UX testing and iteration, publicize official release. |
| 1.1+    |             | Ranged combat, multiple currencies, item durability, flight, underwater, a default new user tutorial area, auctions, banking, embedded mini-games, macros, area instancing, etc... |

### After Version 1.0
We're not stopping at v1. We just mark any issues that we felt were not required for our V1 release as 1.1+.
After the 1.0 public release, we plan to shift our primary focus onto fostering a strong feedback loop with our communities.
We hope many independently developed MUDs will feed back to the core with integration requests, while keeping their own code-bases in a clean enough state to pull from the core as well.
We will all grow and improve at a rate much faster than the previous era of MUDs.
At that time, we will also build a roadmap for a V2.0 release, and continue to develop new features directly for the core.
