using System;

namespace NerdyMishka.Api.KeePass.Package
{
    public interface IRandomByteGenerator
    {
        int Id { get; }

        void Initialize(ReadOnlySpan<byte> key);

        byte[] NextBytes(int count);
    }
}