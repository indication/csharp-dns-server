// // //------------------------------------------------------------------------------------------------- 
// // // <copyright file="IDnsResolver.cs" company="stephbu">
// // // Copyright (c) Steve Butler. All rights reserved.
// // // </copyright>
// // //-------------------------------------------------------------------------------------------------

namespace Dns.Contracts
{
    using System;
    using System.Net;

    /// <summary>Provides domain name resolver capabilities</summary>
    internal interface IDnsResolver : IObserver<Zone>, IHtmlDump
    {
        string GetZoneName();

        uint GetZoneSerial();

        void SubscribeTo(IObservable<Zone> zoneProvider);

        bool TryGetHostEntry(string hostname, ResourceClass resClass, ResourceType resType, out IPHostEntry entry);
    }
}