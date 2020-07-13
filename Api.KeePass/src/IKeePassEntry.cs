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
        string ForegroundColor { get; set; }

        string BackgroundColor { get; set; }

        string OverrideUrl { get; set; }

        IList<string> Tags { get; }

        MemoryProtectedTextDictionary Strings { get; }

        IKeePassAutoType AutoType { get; set; }

        MoveableList<IKeePassEntry> History { get; }

        bool IsHistorical { get; set; }

        MemoryProtectedBytesDictionary Binaries { get; }

        IKeePassEntry CopyTo(IKeePassEntry destination, bool cleanHistory = false);
    }
}