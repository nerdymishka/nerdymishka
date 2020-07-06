using System;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassAutoType
    {
        bool Enabled { get; set; }

        int DataTransferObfuscation { get; set; }

        IKeePassAssociation Association { get; }
    }
}