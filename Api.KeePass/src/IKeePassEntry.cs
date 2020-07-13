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

        bool IsHistorical { get; set; }

        IKeePassAutoType AutoType { get; set; }

        IList<string> Tags { get; }

        MemoryProtectedTextDictionary Strings { get; }

        MemoryProtectedBytesDictionary Binaries { get; }

        MoveableList<IKeePassEntry> History { get; }

        CustomDataDictionary CustomData { get; }

        IKeePassEntry CopyTo(IKeePassEntry destination, bool cleanHistory = false);
    }
}