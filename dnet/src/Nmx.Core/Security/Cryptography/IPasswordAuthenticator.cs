using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    public interface IPasswordAuthenticator : IHashProvider
    {
        int HashSize { get; }

        bool Verify(ReadOnlySpan<byte> value, ReadOnlySpan<byte> hash);
    }
}
