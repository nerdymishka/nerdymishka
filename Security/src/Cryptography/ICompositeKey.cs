using System;
using System.Collections.Generic;

namespace NerdyMishka.Security.Cryptography
{
    public interface ICompositeKey : IEnumerable<ICompositeKeyFragment>, IDisposable
    {
        int Count { get; }

        void Add(ICompositeKeyFragment fragment);

        void Remove(ICompositeKeyFragment fragment);

        void Clear();

        ReadOnlySpan<byte> AssembleKey();
    }
}