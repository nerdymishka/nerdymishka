using System;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassAssociation
    {
        string Window { get; set; }

        string KeystrokeSequence { get; set; }
    }
}