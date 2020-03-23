//-----------------------------------------------------------------------------
// <copyright file="IWheelMudDocumentStorageProvider.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    /// <summary>Interface for supporting different document databases for storing WheelMUD data.</summary>
    public interface IWheelMudDocumentStorageProvider
    {
        string Name { get; }

        IBasicDocumentSession CreateDocumentSession();

        /// <summary>
        /// Expose some debug context to the developer, such as an explorer window at these documents, or a 
        /// document explorer browser app.
        /// </summary>
        void DebugExplore();

        /// <summary>
        /// Prepare the service (prior to allowing any user connections to the server). This is a good place
        /// to kick off some async preparation work if the document storage provider needs some time.
        /// </summary>
        void Prepare();
    }
}
