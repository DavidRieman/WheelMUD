//-----------------------------------------------------------------------------
// <copyright file="TalentFinder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Karak
//   Date      : June 5, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using WheelMUD.Core;
    using WheelMUD.Interfaces;

    public class TalentFinder : IRecomposable
    {
        private static readonly TalentFinder SingletonInstance = new TalentFinder();

        /// <summary>Prevents a default instance of the TalentFinder class from being created.</summary>
        private TalentFinder()
        {
            this.Recompose();
        }

        /// <summary>Gets the singleton instance of the TalentFinder class.</summary>
        public static TalentFinder Instance
        {
            get { return SingletonInstance; }
        }

        public List<Talent> NormalTalents { get; private set; }

        public List<Talent> RacialTalents { get; private set; }

        [ImportMany]
        private List<Talent> AllTalents { get; set; }

        public void Recompose()
        {
            AttributedModelServices.ComposeParts(DefaultComposer.Container, this);

            this.NormalTalents = (from t in this.AllTalents where t.TalentType == TalentType.Normal select t).ToList();
            this.RacialTalents = (from t in this.AllTalents where t.TalentType == TalentType.Racial select t).ToList();
        }
    }
}