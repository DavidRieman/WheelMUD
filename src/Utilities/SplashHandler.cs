//-----------------------------------------------------------------------------
// <copyright file="SplashHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Utilities
{
    public static class SplashHandler
    {
        public static string Get(int number = -1)
        {
            var random = new Random();

            if (number < 0 || number > SplashScreens.Length - 1)
            {
                return SplashScreens[random.Next(0, SplashScreens.Length)];
            }

            return SplashScreens[number];
        }

        private static readonly string[] SplashScreens =
        {
         "<%green%>" + "<%nl%>" +
         " _      ____           ____  _____  _____             __  " + "<%nl%>" +
         "| | /| / / /  ___ ___ / /  |/  / / / / _ \\  ___  ___ / /_" + "<%nl%>" +
         "| |/ |/ / _ \\/ -_) -_) / /|_/ / /_/ / // / / _ \\/ -_) __/" + "<%nl%>" +
         "|__/|__/_//_/\\__/\\__/_/_/  /_/\\____/____(_)_//_/\\__/\\__/ " + "<%nl%>" +
         "<%n%>" + "<%nl%>" +
         $"{GameConfiguration.Copyright}" + "<%nl%>" +
         " " + "<%nl%>" +
         $"Welcome to {GameConfiguration.Name} {GameConfiguration.Version}.",
         
         "<%green%>" + "<%nl%>" +
         " _       _  _                    __    __   __   _     _  _____  " + "<%nl%>" +
         "(_)  _  (_)(_)      ____   ____ (__)  (__)_(__) (_)   (_)(_____) " + "<%nl%>" +
         "(_) (_) (_)(_)__   (____) (____) (_) (_) (_) (_)(_)   (_)(_)  (_)" + "<%nl%>" +
         "(_) (_) (_)(____) (_)_(_)(_)_(_) (_) (_) (_) (_)(_)   (_)(_)  (_)" + "<%nl%>" +
         "(_)_(_)_(_)(_) (_)(__)__ (__)__  (_) (_)     (_)(_)___(_)(_)__(_)" + "<%nl%>" +
         "(__) (__) (_) (_) (____) (____)(___)(_)     (_) (_____) (_____)  " + "<%nl%>" +
         "<%n%>" + "<%nl%>" +
         $"{GameConfiguration.Copyright}" + "<%nl%>" +
         "" + "<%nl%>" +
         $"Welcome to {GameConfiguration.Name} {GameConfiguration.Version}.",
         
         "<%green%>" + "<%nl%>" +
         "Y8b Y8b Y888P 888                     888     e   e     8888 8888 888 88e" + "<%nl%>" +
         "Y8b Y8b Y8P  888 ee   ,e e,   ,e e,  888    d8b d8b    8888 8888 888 888b" + "<%nl%>" +
         "Y8b Y8b Y   888 88b d88 88b d88 88b 888   e Y8b Y8b   8888 8888 888 8888D" + "<%nl%>" +
         "Y8b Y8b    888 888 888   , 888   , 888  d8b Y8b Y8b  8888 8888 888 888P  " + "<%nl%>" +
         "Y8P Y     888 888  'YeeP'  'YeeP' 888 d888b Y8b Y8b 'Y88 88P' 888 88     " + "<%nl%>" +
         "<%n%>" + "<%nl%>" +
         $"{GameConfiguration.Copyright}" + "<%nl%>" +
         "" + "<%nl%>" +
         $"Welcome to {GameConfiguration.Name} {GameConfiguration.Version}.",
         
            "<%green%>" + "<%nl%>" +
            "       @@@@@@      @@@@@@@@                                       " + "<%nl%>" +
            "      @@    @@@  @@@      @@                                      " + "<%nl%>" +
            "      @        @@@@         @    @   @   @  @                    @" + "<%nl%>" +
            "     @@         @@          @@   @  @ @  @  @                    @" + "<%nl%>" +
            "    @@          @@           @   @  @ @  @  @ @@    @@@    @@@   @" + "<%nl%>" +
            "    @@          @@           @   @  @ @  @  @@  @  @   @  @   @  @" + "<%nl%>" +
            "    @@         @@@@@         @   @ @   @ @  @   @  @   @  @   @  @" + "<%nl%>" +
            "    @@      @@@@@@@@@@@     @@   @ @   @ @  @   @  @@@@@  @@@@@  @" + "<%nl%>" +
            "    @@     @@@@@@@@@@@@     @@   @ @   @ @  @   @  @      @      @" + "<%nl%>" +
            "    @@@    @@@@@@@@@@@@@   @@     @     @   @   @  @   @  @   @  @" + "<%nl%>" +
            "     @@   @@@@@@@@@@@@@@@ @@      @     @   @   @   @@@    @@@   @" + "<%nl%>" +
            "      @@@ @@@ @  @  @ @@@@@                                       " + "<%nl%>" +
            "        @@@@@  @ @ @  @@@@             @     @   @     @  @@@@@   " + "<%nl%>" +
            "         @@@    @@@    @@@             @@   @@   @     @  @    @  " + "<%nl%>" +
            "         @@@@ @ @@@ @ @@@@             @@   @@   @     @  @     @ " + "<%nl%>" +
            "         @@@    @@@    @@@             @ @ @ @   @     @  @     @ " + "<%nl%>" +
            "         @@@   @ @ @   @@@             @ @ @ @   @     @  @     @ " + "<%nl%>" +
            "          @@@ @  @  @ @@@              @ @ @ @   @     @  @     @ " + "<%nl%>" +
            "          @@@@   @   @@@@              @ @ @ @   @     @  @     @ " + "<%nl%>" +
            "           @@@@@@@@@@@@@@              @  @  @    @   @   @    @  " + "<%nl%>" +
            "            @@@@@@@@@@@                @  @  @     @@@    @@@@@   " + "<%nl%>" +
            "              @@@@@@                                              " + "<%nl%>" +
            "<%n%>" + "<%nl%>" +
            $"{GameConfiguration.Copyright}" + "<%nl%>" +
            "" + "<%nl%>" +
            $"Welcome to {GameConfiguration.Name} {GameConfiguration.Version}."
        };
    }
}