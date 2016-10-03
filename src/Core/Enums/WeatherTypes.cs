//-----------------------------------------------------------------------------
// <copyright file="WeatherTypes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 6/6/2010
//   Different enums to handle various weather related settings.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    /// <summary>Enumeration of seasons.</summary>
    public enum Seasons
    {
        /// <summary>Spring season.</summary>
        Spring,

        /// <summary>Summer season.</summary>
        Summer,

        /// <summary>Autumn season.</summary>
        Autumn,

        /// <summary>Winter season.</summary>
        Winter
    }

    /// <summary>Enumeration of the different climate types.</summary>
    public enum Climate
    {
        /// <summary>Arctic climate.</summary>
        Artic,

        /// <summary>Sub-arctic climate.</summary>
        SubArtic,

        /// <summary>Temperate climate.</summary>
        Temperate,

        /// <summary>Sub-tropical climate.</summary>
        SubTropical,

        /// <summary>Tropical climate.</summary>
        Tropical,

        /// <summary>Desert climate.</summary>
        Desert,

        /// <summary>Arid climate.</summary>
        Arid,

        /// <summary>Arable climate.</summary>
        Arable,

        /// <summary>Coastal climate.</summary>
        Coastal,

        /// <summary>Rainforest climate.</summary>
        Rainforest
    }

    /// <summary>Enum for weather conditions affecting the visibility outdoors.</summary>
    public enum ExteriorVisibility
    {
        /// <summary>Clear visibility.</summary>
        Clear,

        /// <summary>Cloudy visibility.</summary>
        Cloudy,

        /// <summary>Overcast visibility.</summary>
        Overcast,

        /// <summary>Rainy weather visibility.</summary>
        Raining,

        /// <summary>Stormy weather visibility.</summary>
        Stormy,

        /// <summary>Snowy weather visibility.</summary>
        Snowing,

        /// <summary>Clear night visibility.</summary>
        NightClear,

        /// <summary>Rainy night visibility.</summary>
        NightRaining,

        /// <summary>Snowy night visibility.</summary>
        NightSnowing,

        /// <summary>Foggy visibility.</summary>
        Foggy
    }

    /// <summary>Temperatures are in Celsius.</summary>
    public enum Temperatures
    {
        /// <summary>Low temperature in arctic regions.</summary>
        ArticLow = -25,

        /// <summary>Low temperature in sub-arctic regions.</summary>
        SubArticLow = -10,

        /// <summary>Low temperature in temperate regions.</summary>
        TemperateLow = -5,

        /// <summary>Low temperature in SubTropical regions.</summary>
        SubTropicalLow = 15,

        /// <summary>Low temperature in tropical regions.</summary>
        TropicalLow = 25,

        /// <summary>High temperature in arctic regions.</summary>
        ArticHigh = 10,

        /// <summary>High temperature in sub-arctic regions.</summary>
        SubArticHigh = 15,

        /// <summary>High temperature in Temperate regions.</summary>
        TemperateHigh = 25,

        /// <summary>High temperature in sub-tropical regions.</summary>
        SubTropicalHigh = 30,

        /// <summary>High temperature in tropical regions.</summary>
        TropicalHigh = 40
    }

    /// <summary>Number of hours of rain / year.</summary>
    public enum Precipitation
    {
        /// <summary>Annual hours of rain in desert regions.</summary>
        Desert = 10,

        /// <summary>Annual hours of rain in arid regions.</summary>
        Arid = 30,

        /// <summary>Annual hours of rain in arable regions.</summary>
        Arable = 80,

        /// <summary>Annual hours of rain in coastal regions.</summary>
        Coastal = 150,

        /// <summary>Annual hours of rain in rainforest regions.</summary>
        RainForest = 300
    }
}