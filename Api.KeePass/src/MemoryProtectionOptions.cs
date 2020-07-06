using System;

namespace NerdyMishka.Api.KeePass
{
    public struct MemoryProtectionOptions : IEquatable<MemoryProtectionOptions>
    {
        public bool ProtectTitle { get; set; }

        public bool ProtectUserName { get; set; }

        public bool ProtectPassword { get; set; }

        public bool ProtectUrl { get; set; }

        public bool ProtectNotes { get; set; }

        public static bool operator ==(MemoryProtectionOptions left, MemoryProtectionOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MemoryProtectionOptions left, MemoryProtectionOptions right)
        {
            return !(left == right);
        }

        public static MemoryProtectionOptions Create()
        {
            return new MemoryProtectionOptions()
            {
                ProtectPassword = true,
            };
        }

        public bool Equals(MemoryProtectionOptions other)
        {
            return this.ProtectTitle == other.ProtectTitle &&
                this.ProtectNotes == other.ProtectNotes &&
                this.ProtectPassword == other.ProtectPassword &&
                this.ProtectUrl == other.ProtectUrl &&
                this.ProtectUserName == other.ProtectUserName;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is MemoryProtectionOptions other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash += GetCode(this.ProtectPassword, 1);
            hash += GetCode(this.ProtectTitle, 2);
            hash += GetCode(this.ProtectUserName, 3);
            hash += GetCode(this.ProtectUrl, 4);
            hash += GetCode(this.ProtectNotes, 5);

            return hash;
        }

        private static int GetCode(bool value, int order)
        {
            if (!value)
                return 0;

            return 31 * order;
        }
    }
}