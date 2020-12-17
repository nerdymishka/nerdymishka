using System;

namespace NerdyMishka.Security.Cryptography
{
    public readonly struct HashAlgorithmName : IEquatable<NerdyMishka.Security.Cryptography.HashAlgorithmName>
    {
        private HashAlgorithmName(string name)
        {
            this.Name = name;
        }

        public static HashAlgorithmName MD5 { get;} = new HashAlgorithmName("MD5");

        public static HashAlgorithmName SHA1 { get; } = new HashAlgorithmName("SHA1");

        public static HashAlgorithmName SHA256 { get; } = new HashAlgorithmName("SHA256");

        public static HashAlgorithmName SHA384 { get; } = new HashAlgorithmName("SHA384");

        public static HashAlgorithmName SHA512 { get; } = new HashAlgorithmName("SHA512");

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
            if (obj is HashAlgorithmName name)
                return this.Equals(name);

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