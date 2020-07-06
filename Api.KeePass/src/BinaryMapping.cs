using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    public class BinaryMapping
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public MemoryProtectedBytes Value { get; set; }
    }
}