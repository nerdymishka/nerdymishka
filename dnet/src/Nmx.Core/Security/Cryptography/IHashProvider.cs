using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    public interface IHashProvider
    {
        byte[] ComputeHash(byte[] value);

        bool TryComputeHash(ReadOnlySpan<byte> input, Span<byte> output, out int bytesWritten);
    }
}
