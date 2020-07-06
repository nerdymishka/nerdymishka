using System;

namespace NerdyMishka.Api.KeePass.Package
{
    public interface IRandomByteGeneratorEngine
    {
        int Id { get; }

        void Initialize(ReadOnlySpan<byte> key);

        byte[] NextBytes(int count);
    }
}