using System;
using System.Buffers;

namespace NerdyMishka.Util.Arrays
{
    public static class SpanExtensions
    {
        public static ReadOnlySpan<T> CopyTo<T>(this ReadOnlySpan<T> span, IMemoryOwner<T> owner)
        {
            Check.NotNull(nameof(owner), owner);

            span.CopyTo(owner.Memory.Span);
            return span;
        }

        public static Span<T> CopyTo<T>(this Span<T> span, IMemoryOwner<T> owner)
        {
            Check.NotNull(nameof(owner), owner);

            span.CopyTo(owner.Memory.Span);
            return span;
        }
    }
}