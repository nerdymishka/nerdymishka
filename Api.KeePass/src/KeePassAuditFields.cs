using System;

namespace NerdyMishka.Api.KeePass
{
    public class KeePassAuditFields : IKeePassAuditFields, IEquatable<IKeePassAuditFields>
    {
        public KeePassAuditFields()
        {
            // TODO: figure out if KeePass uses UTC time.
            var now = DateTime.UtcNow;
            this.CreationTime = now;
            this.LastModificationTime = now;
            this.LastAccessTime = now;
            this.ExpiryTime = now;
            this.LocationChanged = now;
            this.Expires = false;
            this.UsageCount = 0;
        }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public DateTime LastAccessTime { get; set; }

        public DateTime ExpiryTime { get; set; }

        public bool Expires { get; set; }

        public int UsageCount { get; set; }

        public DateTime LocationChanged { get; set; }

        public static bool operator ==(KeePassAuditFields left, KeePassAuditFields right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(KeePassAuditFields left, KeePassAuditFields right)
        {
            if (left is null)
                return !(right is null);

            return !left.Equals(right);
        }

        public bool Equals(IKeePassAuditFields other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (this.CreationTime != other.CreationTime)
                return false;

            if (this.LastModificationTime != other.LastModificationTime)
                return false;

            if (this.LastAccessTime != other.LastAccessTime)
                return false;

            if (this.ExpiryTime != other.ExpiryTime)
                return false;

            if (this.LocationChanged != other.LocationChanged)
                return false;

            if (this.Expires != other.Expires)
                return false;

            if (this.UsageCount != other.UsageCount)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is KeePassAuditFields other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17, i = 0;
            hash += i++ * 31 * this.CreationTime.GetHashCode();
            hash += i++ * 31 * this.Expires.GetHashCode();
            hash += i++ * 31 * this.ExpiryTime.GetHashCode();
            hash += i++ * 31 * this.LastAccessTime.GetHashCode();
            hash += i++ * 31 * this.LastModificationTime.GetHashCode();
            hash += i++ * 31 * this.LocationChanged.GetHashCode();
            hash += i++ * 31 * this.UsageCount.GetHashCode();

            return hash;
        }
    }
}