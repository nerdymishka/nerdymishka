using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Api.KeePass
{
    public class KeePassDocument : IKeePassDocument
    {
        private readonly IKeePassPackage package;
        private MoveableList<DeletedObjectInfo> deletedObjects;

        public KeePassDocument(IKeePassPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.RootGroup = new KeePassGroup()
            {
                Name = package.MetaInfo.DatabaseName,
                Package = package,
            };
        }

        public IKeePassPackage Package => this.package;

        public MoveableList<DeletedObjectInfo> DeletedObjects
        {
            get
            {
                if (this.deletedObjects == null)
                    this.deletedObjects = new MoveableList<DeletedObjectInfo>(this.package, this.RootGroup);

                return this.deletedObjects;
            }
        }

        public IKeePassGroup RootGroup { get; protected set; }

        public MoveableList<IKeePassGroup> Groups
        {
            get
            {
                return this.RootGroup.Groups;
            }
        }
    }
}
