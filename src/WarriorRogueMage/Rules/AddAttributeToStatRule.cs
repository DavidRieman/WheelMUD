//-----------------------------------------------------------------------------
// <copyright file="AddAttributeToStatRule.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/12/2012
//   Purpose   : Rule to add the value of a GameAttribute to the value of a GameStat.
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Rules
{
    using System;
    using WarriorRogueMage;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;
    using WheelMUD.Rules;

    /// <summary>Adds the value of a <see cref="GameAttribute" /> to a <see cref="GameStat" />.</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    public class AddAttributeToStatRule<T, R> : GameRule<T, R>
    {
        private static Thing parentThing;
        private static T privateAttrib;
        private static R privateStat;

        public override string RuleKind
        {
            get { return "AddAttributeToStatRule"; }
        }

        public override void Execute(IThing playerThing, string attributeName, string statName)
        {
            parentThing = (Thing)playerThing;
            FindStat(statName);
            FindAttribute(attributeName);
            AddAttributeValueToStatValue();
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

        private static void AddAttributeValueToStatValue()
        {
            var stat = (WRMStat)Convert.ChangeType(privateStat, typeof(WRMStat));
            var attrib = (WRMAttribute)Convert.ChangeType(privateAttrib, typeof(WRMAttribute));
            stat.Increase(attrib.Value, parentThing);
        }
    }
}