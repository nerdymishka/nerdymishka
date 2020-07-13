using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Api.KeePass
{
    public struct DeletedObjectInfo :
        IKeePassChild,
        IEquatable<DeletedObjectInfo>,
        ICloneable<DeletedObjectInfo>,
        IChildCloneable<DeletedObjectInfo>
    {
        public KeePassIdentifier Id { get; set; }

        public DateTime DeletionTime { get; set; }

        public IKeePassPackage Package { get; set; }

        public IKeePassGroup Parent { get; set; }

        public static bool operator ==(DeletedObjectInfo left, DeletedObjectInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DeletedObjectInfo left, DeletedObjectInfo right)
        {
            return !(left == right);
        }

        public bool Equals(DeletedObjectInfo other)
        {
            if (this.Id == other.Id && this.DeletionTime == other.DeletionTime)
                return true;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is DeletedObjectInfo other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() + this.DeletionTime.GetHashCode();
        }

        public DeletedObjectInfo Clone(IKeePassPackage package, IKeePassGroup group)
        {
            return new DeletedObjectInfo()
            {
                DeletionTime = this.DeletionTime,
                Id = this.Id,
                Package = package,
                Parent = group,
            };
        }

        public DeletedObjectInfo Clone()
        {
            return this.Clone(this.Package, this.Parent);
        }
    }
}
