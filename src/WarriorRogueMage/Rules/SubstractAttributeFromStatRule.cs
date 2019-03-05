﻿//-----------------------------------------------------------------------------
// <copyright file="SubstractAttributeFromStatRule.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Rule to subtract the value of a GameAttribute from the value of a GameStat.
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Rules
{
    using System;
    using WarriorRogueMage;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;
    using WheelMUD.Rules;

    /// <summary>Subtract the value of a <see cref="GameAttribute"/> from a <see cref="GameStat"/>.</summary>
    public class SubstractStatFromAttributeRule<T, R> : GameRule<T, R>
    {
        private static Thing parentThing;
        private static T privateAttrib;
        private static R privateStat;

        public override string RuleKind
        {
            get { return "SubstractAttributeFromStatRule"; }
        }

        public override void Execute(IThing playerThing, string attributeName, string statName)
        {
            parentThing = (Thing)playerThing;

            FindStat(statName);
            FindAttribute(attributeName);
            SubtractAttributeValueFromStatValue();
        }

        public override ValidationResult Validate(T value)
        {
            return ValidationResult.Success;
        }

        private static void FindAttribute(string attributeNameToFind)
        {
            var attributeToFind = parentThing.FindGameAttribute(attributeNameToFind);

            var attrib = (WRMAttribute)Convert.ChangeType(attributeToFind, typeof(WRMAttribute));

            privateAttrib = (T)Convert.ChangeType(parentThing.FindGameAttribute(attrib.Name), typeof(T));
        }

        private static void FindStat(string statNameToFind)
        {
            var statToFind = parentThing.FindGameStat(statNameToFind);

            var stat = (WRMStat)Convert.ChangeType(statToFind, typeof(WRMStat));

            privateStat = (R)Convert.ChangeType(parentThing.FindGameStat(stat.Name), typeof(R));
        }

        private static void SubtractAttributeValueFromStatValue()
        {
            var stat = (WRMStat)Convert.ChangeType(privateStat, typeof(WRMStat));
            var attrib = (WRMAttribute)Convert.ChangeType(privateAttrib, typeof(WRMAttribute));

            stat.Decrease(attrib.Value, parentThing);
        }
    }
}