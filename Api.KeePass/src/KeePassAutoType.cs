using System;

namespace NerdyMishka.Api.KeePass
{
    public class KeePassAutoType : IKeePassAutoType, IEquatable<KeePassAutoType>
    {
        private IKeePassAssociation association;

        public bool Enabled { get; set; }

        public int DataTransferObfuscation { get; set; }

        public IKeePassAssociation Association
        {
            get
            {
                if (this.association == null)
                    this.association = new KeePassAssociation();
                return this.association;
            }

            internal protected set
            {
                this.association = value;
            }
        }

        public static bool operator ==(KeePassAutoType left, KeePassAutoType right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(KeePassAutoType left, KeePassAutoType right)
        {
            if (left is null)
                return !(right is null);

            return !left.Equals(right);
        }

        public bool Equals(KeePassAutoType other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return this.Enabled == other.Enabled &&
                this.DataTransferObfuscation == other.DataTransferObfuscation &&
                this.association == other.association;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is KeePassAutoType other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash += 31 * this.Enabled.GetHashCode();
            hash += 31 * 2 * this.DataTransferObfuscation.GetHashCode();

            if (this.association != null)
                hash += this.association.GetHashCode();

            return hash;
        }
    }
}