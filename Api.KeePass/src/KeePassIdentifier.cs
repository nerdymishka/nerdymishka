using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NerdyMishka.Util.Collections;

namespace NerdyMishka.Api.KeePass
{
    [SuppressMessage("Microsoft.Usage", "CA2225:Operator overloads have named alternates", Justification = "By Design")]
    public struct KeePassIdentifier : IEquatable<KeePassIdentifier>
    {
        private readonly byte[] uuid;

        public KeePassIdentifier(byte[] uuid)
        {
            if (uuid is null || uuid.Length == 0)
                throw new ArgumentNullException(nameof(uuid));

            if (uuid.Length != 16)
                throw new ArgumentException("uuid must have 16 bytes.", nameof(uuid));

            this.uuid = uuid;
        }

        public static KeePassIdentifier Empty => new KeePassIdentifier(
            new byte[]
            {
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
            });

        public bool IsEmpty
        {
            get
            {
                return Empty.uuid.EqualTo(this.uuid);
            }
        }

        public static implicit operator KeePassIdentifier(byte[] uuid)
        {
            return new KeePassIdentifier(uuid);
        }

        public static bool operator ==(KeePassIdentifier a, KeePassIdentifier b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(KeePassIdentifier a, KeePassIdentifier b)
        {
            return !a.Equals(b);
        }

        public static KeePassIdentifier NewIdentifier()
        {
            var bytes = KeePassIdentifierFactory.Generate();
            return new KeePassIdentifier(bytes);
        }

        public bool Equals(KeePassIdentifier other)
        {
            return this.uuid.EqualTo(other.uuid);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<byte>.Default;

            unchecked
            {
                if (this.uuid == null)
                {
                    return 0;
                }

                int hash = 17;
                foreach (byte item in this.uuid)
                {
                    hash = (hash * 3) + comparer.GetHashCode(item);
                }

                return hash;
            }
        }

        public override string ToString()
        {
            return Convert.ToBase64String(this.uuid);
        }
    }
}