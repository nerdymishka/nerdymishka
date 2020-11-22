using System;
using System.Buffers;
using System.IO;

namespace NerdyMishka.Util.IO
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, ReadOnlyMemory<byte> memory)
        {
            Check.ArgNotNull(stream, nameof(stream));
            stream.Write(memory.Span);
        }
    }
}