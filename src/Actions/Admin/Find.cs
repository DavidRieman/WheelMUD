//-----------------------------------------------------------------------------
// <copyright file="Find.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command that allows an admin to locate an entity.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("find", CommandCategory.Builder)]
    [ActionDescription("Finds Things that match the specified argument.")]
    [ActionSecurity(SecurityRole.minorBuilder)]
    public class Find : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument,
            CommonGuards.InitiatorMustBeAPlayer
        };

        private static readonly ThingManager _thingManager = ThingManager.Instance;

        public override void Execute(ActionInput actionInput)
        {
            // To determine what type of find to use.
            var findType = actionInput.Params[0];
            // Find params from rest of Params array.
            var findParams = actionInput.Params.Skip(1).ToArray();
            // Set this to true if we find anyThing.
            var thingFound = false;

            if ("id".Contains(findType.ToLower()))
            {
                if (TryFindThingByID(string.Join(" ", findParams), out var thing))
                {
                    WriteOneThing(actionInput, thing, true);
                    thingFound = true;
                }
            }
            else if ("keyword".Contains(findType.ToLower()))
            {
                thingFound = TryFindThingsByKeyword(findParams, out var thingsList);
                WriteThingList(actionInput, thingsList);
            }
            else if ("name".Contains(findType.ToLower()))
            {
                thingFound = TryFindThingsByName(findParams, out var thingsList);
                WriteThingList(actionInput, thingsList);
            }
            else
            {
                actionInput.Session.WriteLine($" First argument must be 'id', 'keyword' or 'name'.");
                return;
            }

            if (!thingFound)
            {
                actionInput.Session.WriteLine(
                    $" No Things were found matching the given criteria: {string.Join(" ", actionInput.Params)}");
            }
        }

        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }

        private static bool TryFindThingByID(string ThingID, out Thing thing)
        {
            thing = _thingManager.FindThing(ThingID);

            return thing != null;
        }

        private static bool TryFindThingsByKeyword(string[] keywords, out IList<Thing> thingsList)
        {
            thingsList = _thingManager.Find((thing) =>
            {
                return thing.KeyWords.Intersect(keywords, StringComparer.InvariantCultureIgnoreCase).Any();
            });

            return thingsList.Count > 0;
        }

        private static bool TryFindThingsByName(string[] keywords, out IList<Thing> thingsList)
        {
            thingsList = _thingManager.Find(thing =>
            {
                return keywords.All(keyword => thing.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));
            });

            return thingsList.Count > 0;
        }

        private static void WriteOneThing(ActionInput input, Thing thing, bool isLast)
        {
            input.Session.WriteLine($" [{thing.Id}] - {thing.Name}", isLast);
        }

        private static void WriteThingList(ActionInput input, IList<Thing> thingsList)
        {
            if (thingsList.Count > 0)
            {
                if (thingsList.Count > 1)
                {
                    input.Session.WriteLine($"A total of {thingsList.Count} Things were found:", false);
                }

                var totalToWrite = thingsList.Count;

                if (totalToWrite > 100)
                {
                    input.Session.WriteLine($"Too many Things found! Only the first 100 Things will be showed.");
                    totalToWrite = 100;
                }

                for (var thingIndex = 0; thingIndex < totalToWrite; thingIndex++)
                {
                    var listThing = thingsList[thingIndex];
                    WriteOneThing(input, listThing, listThing == thingsList[thingsList.Count - 1]);
                }
            }
        }
    }
}