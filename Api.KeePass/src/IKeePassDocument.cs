using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassDocument
    {
        IKeePassPackage Package { get; }

        MoveableList<DeletedObjectInfo> DeletedObjects { get; }

        MoveableList<IKeePassGroup> Groups { get; }

        IKeePassGroup RootGroup { get; }
    }
}
