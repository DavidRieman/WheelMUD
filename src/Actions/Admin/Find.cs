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
            CommonGuards.RequiresAtLeastOneArgument
        };

        private static readonly ThingManager _thingManager = ThingManager.Instance;

        public override void Execute(ActionInput actionInput)
        {
            var thingsList = new List<Thing>();

            foreach (var param in actionInput.Params)
            {
                if (!FindThingByID(param, ref thingsList))
                {
                    FindThingsByName(param, ref thingsList);
                }
            }

            if (thingsList.Count > 0)
            {
                if (thingsList.Count > 1)
                {
                    actionInput.Session?.WriteLine($" A total of {thingsList.Count} Things were found:", false);
                }

                foreach (var thing in thingsList)
                {
                    actionInput.Session?.WriteLine($" [{thing.Id}] - {thing.Name}", thing == thingsList[thingsList.Count - 1]);
                }
            }
            else
            {
                actionInput.Session?.WriteLine(
                    $" No Things were found matching the given criteria: {string.Join(" ", actionInput.Params)}");
            }
        }

        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }

        private static bool FindThingByID(string ThingID, ref List<Thing> thingsList)
        {
            var thing = _thingManager.FindThing(ThingID);

            if (thing != null)
            {
                thingsList.Add(thing);
                return true;
            }

            return false;
        }

        private static void FindThingsByName(string ThingName, ref List<Thing> thingsList)
        {
            var things = _thingManager.Find(
                    thing => thing.Name.Contains(ThingName, StringComparison.InvariantCultureIgnoreCase));

            if (things.Count > 0)
            {
                thingsList.AddRange(things);
            }
        }
    }
}