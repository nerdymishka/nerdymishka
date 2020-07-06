using System;
using System.Collections.Generic;
using NerdyMishka.Util.Collections;

namespace NerdyMishka.Api.KeePass
{
    public struct KeePassIcon : IEquatable<KeePassIcon>
    {
        private byte[] data;

        public KeePassIdentifier Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">The left value.</param>
        /// <param name="b">The right value.</param>
        /// <returns><c>true</c> if equal; otherwise, false.</returns>
        public static bool operator ==(KeePassIcon a, KeePassIcon b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Not equal operator.
        /// </summary>
        /// <param name="a">The left value.</param>
        /// <param name="b">The right value.</param>
        /// <returns><c>true</c> if not equal; otherwise, false.</returns>
        public static bool operator !=(KeePassIcon a, KeePassIcon b)
        {
            return !a.Equals(b);
        }

        public void SetIcon(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                this.data = Array.Empty<byte>();
                return;
            }

            this.data = Convert.FromBase64String(data);
        }

        public void SetIcon(byte[] data)
        {
            if (data is null || data.Length == 0)
            {
                this.data = Array.Empty<byte>();
                return;
            }

            this.data = new byte[data.Length];
            Array.Copy(data, this.data, data.Length);
        }

        public byte[] ToBytes()
        {
            return this.data;
        }

        public bool Equals(KeePassIcon other)
        {
            return this.Id.Equals(other.Id) &&
                this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                this.data.EqualTo(other.data);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is KeePassIcon other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<byte>.Default;

            unchecked
            {
#if NETSTANDARD2_0
                int hash = this.Id.GetHashCode() + this.Name.GetHashCode();
#else
                int hash = this.Id.GetHashCode() + this.Name.GetHashCode(StringComparison.OrdinalIgnoreCase);
#endif

                if (this.data == null || this.data.Length == 0)
                    return hash;

                foreach (byte item in this.data)
                {
                    hash = (hash * 3) + comparer.GetHashCode(item);
                }

                return hash;
            }
        }

        public override string ToString()
        {
            return Convert.ToBase64String(this.data);
        }
    }
}