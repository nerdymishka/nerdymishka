using System;
using System.Buffers;
using System.IO;

namespace NerdyMishka.Text.DotEnv
{
    public static class Extensions
    {
#if NETSTANDARD2_0
        public static int ReadBlock(this TextReader reader, Span<char> span)
        {
            if (reader is null)
                return 0;

            var set = ArrayPool<char>.Shared.Rent(span.Length);
            var read = reader.ReadBlock(set, 0, set.Length);
            set.CopyTo(span);
            ArrayPool<char>.Shared.Return(set);
            return read;
        }
#endif
    }
}