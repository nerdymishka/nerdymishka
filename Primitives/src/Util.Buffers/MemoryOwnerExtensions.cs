using System;
using System.Buffers;

namespace NerdyMishka.Util.Buffers
{
    public static class MemoryOwnerExtensions
    {
        public static IMemoryOwner<T> CopyTo<T>(this IMemoryOwner<T> owner, Span<T> span)
        {
            Check.NotNull(nameof(owner), owner);
            owner.Memory.Span.CopyTo(span);
            return owner;
        }

        public static IMemoryOwner<T> CopyFrom<T>(this IMemoryOwner<T> owner, Span<T> span)
        {
            Check.NotNull(nameof(owner), owner);
            span.CopyTo(owner.Memory.Span);
            return owner;
        }

        public static IMemoryOwner<T> CopyFrom<T>(this IMemoryOwner<T> owner, ReadOnlySpan<T> span)
        {
            Check.NotNull(nameof(owner), owner);
            span.CopyTo(owner.Memory.Span);
            return owner;
        }
    }
}