using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassNode
    {
        KeePassIdentifier Id { get; set; }

        int IconId { get; set; }

        IKeePassAuditFields AuditFields { get; }

        string Name { get; set; }

        string Notes { get; set; }
    }
}
