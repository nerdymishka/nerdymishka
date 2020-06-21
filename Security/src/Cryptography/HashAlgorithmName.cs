using System;

namespace NerdyMishka.Security.Cryptography
{
    public struct HashAlgorithmName : IEquatable<NerdyMishka.Security.Cryptography.HashAlgorithmName>
    {
        private HashAlgorithmName(string name)
        {
            this.Name = name;
        }

        public static HashAlgorithmName MD5 { get; private set; } = new HashAlgorithmName("MD5");

        public static HashAlgorithmName SHA1 { get; private set; } = new HashAlgorithmName("SHA1");

        public static HashAlgorithmName SHA256 { get; private set; } = new HashAlgorithmName("SHA256");

        public static HashAlgorithmName SHA384 { get; private set; } = new HashAlgorithmName("SHA384");

        public static HashAlgorithmName SHA512 { get; private set; } = new HashAlgorithmName("SHA512");

        public string Name { get; }

        public static bool operator ==(HashAlgorithmName left, HashAlgorithmName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HashAlgorithmName left, HashAlgorithmName right)
        {
            return !left.Equals(right);
        }

        public bool Equals(HashAlgorithmName other)
        {
            return this.Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is HashAlgorithmName)
                return this.Equals((HashAlgorithmName)obj);

            return false;
        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            return this.Name.GetHashCode() * 31;
#else
            return this.Name.GetHashCode(StringComparison.OrdinalIgnoreCase) * 31;
#endif
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}