//-----------------------------------------------------------------------------
// <copyright file="IWheelMudDocumentStorageProvider.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Interface for supporting different document databases for storing WheelMUD data.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using System.Data;

    public interface IWheelMudDocumentStorageProvider
    {
        string Name { get; }
    }
}
