using System.IO;
using System.Buffers;
using System;

namespace NerdyMishka.Text.DotEnv
{
    public static class Extensions
    {
#if NETSTANDARD2_0
        public static int ReadBlock(this TextReader reader, Span<char> span)
        {
            var set = ArrayPool<char>.Shared.Rent(span.Length);
            var read = reader.ReadBlock(set, 0, set.Length);
            set.CopyTo(span);
            ArrayPool<char>.Shared.Return(set);
            return read;
        }
#endif 

    }
}