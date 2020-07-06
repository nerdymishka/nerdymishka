using System;

namespace NerdyMishka.Api.KeePass
{
    /// <summary>
    /// Represents a binary file stored in KeePass.
    /// </summary>
    public struct BinaryInfo : IEquatable<BinaryInfo>
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        /// <value>The Id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the binary is compressed or
        /// not.
        /// </summary>
        /// <value>Is Compressed.</value>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="a">The left value.</param>
        /// <param name="b">The right value.</param>
        /// <returns><c>true</c> if equal; otherwise, false.</returns>
        public static bool operator ==(BinaryInfo a, BinaryInfo b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Not equal operator.
        /// </summary>
        /// <param name="a">The left value.</param>
        /// <param name="b">The right value.</param>
        /// <returns><c>true</c> if not equal; otherwise, false.</returns>
        public static bool operator !=(BinaryInfo a, BinaryInfo b)
        {
            return !a.Equals(b);
        }

        public bool Equals(BinaryInfo other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is BinaryInfo other)
                return this.Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return this.Id ^ (this.IsCompressed ? 1 : 0);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}