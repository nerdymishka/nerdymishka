using System;
using System.Buffers;

namespace NerdyMishka.Util.Buffers
{
    public static class MemoryOwnerExtensions
    {
        public static ReadOnlySpan<T> CopyTo<T>(this ReadOnlySpan<T> span, IMemoryOwner<T> owner)
        {
            Check.ArgNotNull(owner, nameof(owner));

            span.CopyTo(owner.Memory.Span);
            return span;
        }

        public static Span<T> CopyTo<T>(this Span<T> span, IMemoryOwner<T> owner)
        {
            Check.ArgNotNull(owner, nameof(owner));

            span.CopyTo(owner.Memory.Span);
            return span;
        }

        public static IMemoryOwner<T> CopyTo<T>(this IMemoryOwner<T> owner, Span<T> span)
        {
            Check.ArgNotNull(owner, nameof(owner));

            owner.Memory.Span.CopyTo(span);
            return owner;
        }

        public static IMemoryOwner<T> CopyFrom<T>(this IMemoryOwner<T> owner, Span<T> span)
        {
            Check.ArgNotNull(owner, nameof(owner));
            span.CopyTo(owner.Memory.Span);
            return owner;
        }

        public static IMemoryOwner<T> CopyFrom<T>(this IMemoryOwner<T> owner, ReadOnlySpan<T> span)
        {
            Check.ArgNotNull(owner, nameof(owner));

            span.CopyTo(owner.Memory.Span);
            return owner;
        }
    }
}