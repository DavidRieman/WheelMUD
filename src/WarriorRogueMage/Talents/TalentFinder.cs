//-----------------------------------------------------------------------------
// <copyright file="TalentFinder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Utilities.Interfaces;

namespace WarriorRogueMage
{
    public class TalentFinder : IRecomposable
    {
        /// <summary>Prevents a default instance of the TalentFinder class from being created.</summary>
        private TalentFinder()
        {
            Recompose();
        }

        /// <summary>Gets the singleton instance of the TalentFinder class.</summary>
        public static TalentFinder Instance { get; } = new TalentFinder();

        public List<Talent> NormalTalents { get; private set; }

        public List<Talent> RacialTalents { get; private set; }

        [ImportMany]
        private List<Talent> AllTalents { get; set; }

        public void Recompose()
        {
            AttributedModelServices.ComposeParts(DefaultComposer.Container, this);

            NormalTalents = (from t in AllTalents where t.TalentType == TalentType.Normal select t).ToList();
            RacialTalents = (from t in AllTalents where t.TalentType == TalentType.Racial select t).ToList();
        }
    }
}