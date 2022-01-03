//-----------------------------------------------------------------------------
// <copyright file="WearLocations.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>Depicts the different places you can wear a piece of equipment.</summary>
    [Flags]
    public enum WearLocations : ulong
    {
        /// <summary>No wear location.</summary>
        None = 0,

        /// <summary>Wear location is on top of the player's head: Crown, Helmet.</summary>
        Head = 1,

        /// <summary>Wear location is the player's face: Glasses, Monocle.</summary>
        Face = 1 << 1,

        /// <summary>Wear location is the player's left ear.</summary>
        EarLeft = 1 << 2,

        /// <summary>Wear location is the player's right ear.</summary>
        EarRight = 1 << 3,

        /// <summary>Wear location is the player's ears: Earmuffs.</summary>
        Ears = EarLeft | EarRight,

        /// <summary>Wear location is the player's neck.</summary>
        Neck = 1 << 4,

        /// <summary>Wear location is the player's shoulders.</summary>
        Shoulders = 1 << 5,

        /// <summary>Wear location is the player's upper body.</summary>
        UpperBody = 1 << 6,

        /// <summary>Wear location is the player's upper body, under.</summary>
        UpperBodyUnderwear = 1 << 7,

        /// <summary>Wear location is the player's lower body: Pants.</summary>
        LowerBody = 1 << 8,

        /// <summary>Wear location is the player's lower body, under: Underwear, thong.</summary>
        LowerBodyUnderwear = 1 << 9,

        /// <summary>Wear location is the player's torso.</summary>
        Torso = UpperBody | LowerBody,

        /// <summary>Wear location is the player's arms.</summary>
        ArmLeft = 1 << 10,

        /// <summary>Wear location is the player's arms.</summary>
        ArmRight = 1 << 11,

        /// <summary>Wear location is the player's arms.</summary>
        Arms = ArmLeft | ArmRight,

        /// <summary>Wear location is the player's left elbow.</summary>
        ElbowLeft = 1 << 12,

        /// <summary>Wear location is the player's right elbow.</summary>
        ElbowRight = 1 << 13,

        /// <summary>Wear location is both the player's elbows.</summary>
        Elbows = ElbowLeft | ElbowRight,

        /// <summary>Wear location is the player's forearm: Shields are bound to the forearm and held in hand in real life.</summary>
        ForearmRight = 1 << 14,

        /// <summary>Wear location is the player's left forearm: Shields are bound to the forearm and help in hand in real life.</summary>
        ForearmLeft = 1 << 15,

        /// <summary>Wear location is both the player's forearms.</summary>
        Forearms = ForearmLeft | ForearmRight,

        /// <summary>Wear location is the player's left wrist: Bracelet, for example.</summary>
        WristLeft = 1 << 16,

        /// <summary>Wear location is the player's right wrist: Bracelet, for example.</summary>
        WristRight = 1 << 17,

        /// <summary>Wear location is both the player's wrist: Handcuffs, for example.</summary>
        Wrists = WristLeft | WristRight,

        /// <summary>Wear location is the player's right hand: Catchers mitt.</summary>
        HandRight = 1 << 18,

        /// <summary>Wear location is the player's left hand: Left handed catchers mitt.</summary>
        HandLeft = 1 << 19,

        /// <summary>Wear location is both the player's hands: A set of gloves.</summary>
        Hands = HandLeft | HandRight,

        /// <summary>Wear location is the player's left finger slot.</summary>
        RingFingerLeft = 1 << 20,

        /// <summary>Wear location is the player's right finger slot.</summary>
        RingFingerRight = 1 << 21,

        /// <summary>Wear location is both the player's finger slots.</summary>
        RingFingers = RingFingerLeft | RingFingerRight,

        /// <summary>Wear location is the player's waist: Belt.</summary>
        Waist = 1 << 22,

        /// <summary>Wear location is the player's left leg.</summary>
        LegLeft = 1 << 23,

        /// <summary>Wear location is the player's right leg.</summary>
        LegRight = 1 << 24,

        /// <summary>Wear location is the player's legs.</summary>
        Legs = LegRight | LegLeft,

        /// <summary>Wear location is the player's left knee.</summary>
        KneeLeft = 1 << 25,

        /// <summary>Wear location is the player's right knee.</summary>
        KneeRight = 1 << 26,

        /// <summary>Wear location is the player's knees.</summary>
        Knees = KneeLeft | KneeRight,

        /// <summary>Wear location is the player's shin.</summary>
        ShinLeft = 1 << 27,

        /// <summary>Wear location is the player's shin.</summary>
        ShinRight = 1 << 28,

        /// <summary>Wear location is the player's shins.</summary>
        Shins = ShinLeft | ShinRight,

        /// <summary>Wear location is the player's right ankle.</summary>
        AnkleRight = 1 << 29,

        /// <summary>Wear location is the player's left ankle.</summary>
        AnkleLeft = 1 << 30,

        /// <summary>Wear location is the player's ankles.</summary>
        Ankles = AnkleLeft | AnkleRight,

        /// <summary>Wear location is the player's left foot.</summary>
        FootLeft = 1ul << 31,

        /// <summary>Wear location is the player's right foot.</summary>
        FootRight = 1ul << 32,

        /// <summary>Wear location is the player's feet.</summary>
        Feet = FootLeft | FootRight,

        /// <summary>Wear location surrounds the player.</summary>
        Surrounding = 1ul << 33,

        /// <summary>All of the wearable locations (including future or game-specific extensions).</summary>
        All = 0xFFFFFFFFFFFFFFFF,

        // Game implementations can extend this without modifying Core code, by providing their own constants that cast to WearLocations.
        // For example, supposing your game has four-legged characters and you want wearable equipment locations for those legs:
        //   public const WearLocations WearLocationFrontLeftLeg = (WearLocations)(1ul << 40);
        //   public const WearLocations WearLocationFrontRightLeg = (WearLocations)(1ul << 41);
        //   public const WearLocations WearLocationFrontLegs = WearLocationFrontLeftLeg | WearLocationFrontRightLeg;
    }
}