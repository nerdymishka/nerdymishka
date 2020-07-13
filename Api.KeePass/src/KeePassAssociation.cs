using System;

namespace NerdyMishka.Api.KeePass
{
    public class KeePassAssociation : IKeePassAssociation, IEquatable<KeePassAssociation>
    {
        public string Window { get; set; }

        public string KeystrokeSequence { get; set; }

        public static bool operator ==(KeePassAssociation left, KeePassAssociation right)
        {
            if (left is null)
                return right is null;

            return left.Equals(false);
        }

        public static bool operator !=(KeePassAssociation left, KeePassAssociation right)
        {
            if (left is null)
                return !(right is null);

            return !left.Equals(right);
        }

        public bool Equals(KeePassAssociation other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return this.Window == other.Window &&
                this.KeystrokeSequence == other.KeystrokeSequence;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is KeePassAssociation other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            if (this.Window != null)
                hash += 31 * this.Window.GetHashCode();

            if (this.KeystrokeSequence != null)
                hash += 31 * this.KeystrokeSequence.GetHashCode();

            return hash;
        }
    }
}