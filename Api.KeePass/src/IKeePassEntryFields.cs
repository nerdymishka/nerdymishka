using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Api.KeePass
{
    [SuppressMessage(
        "Microsoft.Design",
        "CA1056:URI properties should not be strings",
        Justification = "By Design. Urls could be malformed by the user.")]
    public interface IKeePassEntryFields
    {
        string Title { get; set; }

        string Url { get; set; }

        string UserName { get; set; }

        string Notes { get; set; }

        IList<string> Tags { get; }
    }
}
