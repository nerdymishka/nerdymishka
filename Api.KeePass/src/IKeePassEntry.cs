using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    [SuppressMessage(
        "Microsoft.Design",
        "CA1056:URI properties should not be strings",
        Justification = "By Design. Urls could be malformed by the user.")]
    public interface IKeePassEntry : IKeePassNode
    {
        IKeePassEntryFields Fields { get; }

        IKeePassPassword Password { get; set; }

        string ForegroundColor { get; set; }

        string BackgroundColor { get; set; }

        string OverrideUrl { get; set; }

        string UserName { get; set; }

        string Url { get; set; }

        IList<string> Tags { get; }

        IList<MemoryProtectedText> Strings { get; }

        IKeePassAutoType AutoType { get; set; }

        IList<IKeePassEntry> History { get; }

        bool IsHistorical { get; set; }

        IList<MemoryProtectedBytes> Binaries { get; }

        bool PreventAutoCreate { get; set; }

        KeePassIdentifier CustomIconUuid { get; set; }

        IKeePassEntry CopyTo(IKeePassEntry destination, bool cleanHistory = false);
    }
}