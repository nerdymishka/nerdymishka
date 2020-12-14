using System;

namespace NerdyMishka.Security
{
    public interface ICompositeKeyFragment : IDisposable
    {
        ReadOnlySpan<byte> ToReadOnlySpan();
    }
}