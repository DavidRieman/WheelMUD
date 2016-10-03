//-----------------------------------------------------------------------------
// <copyright file="WearLocations.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Enum of accepted item wear locations that are usable.
//   Edited by Feverdream @ 4/24/2010 to update bit indexes and slots to include left/right distinctions.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    using System;

    /// <summary>All of the different places you can wear a piece of equipment.</summary>
    [Flags]
    public enum WearLocations : ulong
    {
        /// <summary>Wear location is on top of the player's head: Crown, Helmet.</summary>
        Head = 1,

        /// <summary>Wear location is the player's face: Glasses, Monocle.</summary>
        Face = 2,

        /// <summary>Wear location is the player's left ear.</summary>
        EarLeft = 4,

        /// <summary>Wear location is the player's right ear.</summary>
        EarRight = 8,

        /// <summary>Wear location is the player's ears: Earmuffs.</summary>
        Ears = EarLeft | EarRight,

        /// <summary>Wear location is the player's neck.</summary>
        Neck = 16,

        /// <summary>Wear location is the player's shoulders.</summary>
        Shoulders = 32,

        /// <summary>Wear location is the player's upper body.</summary>
        UpperBody = 64,

        /// <summary>Wear location is the player's upper body, under.</summary>
        UpperBodyUnderwear = 128,

        /// <summary>Wear location is the player's lower body: Pants.</summary>
        LowerBody = 256,

        /// <summary>Wear location is the player's lower body, under: Underwear, thong.</summary>
        LowerBodyUnderwear = 512,

        /// <summary>Wear location is the player's torso.</summary>
        Torso = UpperBody | LowerBody,

        /// <summary>Wear location is the player's arms.</summary>
        ArmLeft = 1024,

        /// <summary>Wear location is the player's arms.</summary>
        ArmRight = 2048,

        /// <summary>Wear location is the player's arms.</summary>
        Arms = ArmLeft | ArmRight,

        /// <summary>Wear location is the player's left elbow.</summary>
        ElbowLeft = 4096,

        /// <summary>Wear location is the player's right elbow.</summary>
        ElbowRight = 8192,

        /// <summary>Wear location is both the player's elbows.</summary>
        Elbows = ElbowLeft | ElbowRight,

        /// <summary>Wear location is the player's forearm: Shields are bound to the forearm and held in hand in real life.</summary>
        ForearmRight = 16384,

        /// <summary>Wear location is the player's left forearm: Shields are bound to the forearm and help in hand in real life.</summary>
        ForearmLeft = 32768,

        /// <summary>Wear location is both the player's forearms.</summary>
        Forearms = ForearmLeft | ForearmRight,

        /// <summary>Wear location is the player's left wrist: Bracelet, for example.</summary>
        WristLeft = 65536,

        /// <summary>Wear location is the player's right wrist: Bracelet, for example.</summary>
        WristRight = 131072,

        /// <summary>Wear location is both the player's wrist: Handcuffs, for example.</summary>
        Wrists = WristLeft | WristRight,

        /// <summary>Wear location is the player's right hand: Catchers mitt.</summary>
        HandRight = 262144,

        /// <summary>Wear location is the player's left hand: Left handed catchers mitt.</summary>
        HandLeft = 524288,

        /// <summary>Wear location is both the player's hands: A set of gloves.</summary>
        Hands = HandLeft | HandRight,

        /// <summary>Wear location is the player's left finger slot.</summary>
        RingFingerLeft = 1048576,

        /// <summary>Wear location is the player's right finger slot.</summary>
        RingFingerRight = 2097152,

        /// <summary>Wear location is both the player's finger slots.</summary>
        RingFingers = RingFingerLeft | RingFingerRight,

        /// <summary>Wear location is the player's waist: Belt.</summary>
        Waist = 4194304,

        /// <summary>Wear location is the player's left leg.</summary>
        LegLeft = 8388608,

        /// <summary>Wear location is the player's right leg.</summary>
        LegRight = 16777216,

        /// <summary>Wear location is the player's legs.</summary>
        Legs = LegRight | LegLeft,

        /// <summary>Wear location is the player's left knee.</summary>
        KneeLeft = 33554432,

        /// <summary>Wear location is the player's right knee.</summary>
        KneeRight = 67108864,

        /// <summary>Wear location is the player's knees.</summary>
        Knees = KneeLeft | KneeRight,

        /// <summary>Wear location is the player's shin.</summary>
        ShinLeft = 134217728,

        /// <summary>Wear location is the player's shin.</summary>
        ShinRight = 268435456,

        /// <summary>Wear location is the player's shins.</summary>
        Shins = ShinLeft | ShinRight,

        /// <summary>Wear location is the player's right ankle.</summary>
        AnkleRight = 536870912,

        /// <summary>Wear location is the player's left ankle.</summary>
        AnkleLeft = 1073741824,

        /// <summary>Wear location is the player's ankles.</summary>
        Ankles = AnkleLeft | AnkleRight,

        /// <summary>Wear location is the player's left foot.</summary>
        FootLeft = 2147483648,

        /// <summary>Wear location is the player's right foot.</summary>
        FootRight = 4294967296,

        /// <summary>Wear location is the player's feet.</summary>
        Feet = FootLeft | FootRight,

        /// <summary>Wear location surrounds the player.</summary>
        Surrounding = 8589934592
    }
}