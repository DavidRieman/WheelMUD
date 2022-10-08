﻿//-----------------------------------------------------------------------------
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
        private const int MaxThings = 100;

        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument,
            CommonGuards.InitiatorMustBeAPlayer
        };

        private static readonly ThingManager _thingManager = ThingManager.Instance;

        public override void Execute(ActionInput actionInput)
        {
            if (TryFindThingByID(actionInput.Params[0], out var thing))
            {
                WriteOneThing(actionInput, thing, true);
            }
            else if (TryFindThingsByKeyword(actionInput.Params, out var thingsList))
            {
                WriteThingList(actionInput, thingsList);
            }
            else if (TryFindThingsByName(actionInput.Params, out thingsList))
            {
                WriteThingList(actionInput, thingsList);
            }
            else
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

                if (totalToWrite > MaxThings)
                {
                    input.Session.WriteLine($"Too many Things found! Showing only the first {MaxThings} Things.");
                    totalToWrite = MaxThings;
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