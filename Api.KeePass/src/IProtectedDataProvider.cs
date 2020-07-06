using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Api.KeePass
{
    public interface IProtectedDataProvider
    {
        ReadOnlySpan<byte> ProtectData(ReadOnlySpan<byte> userData, ReadOnlySpan<byte> optionalEntropy, bool isLocalMachine = false);

        ReadOnlySpan<byte> UnprotectData(ReadOnlySpan<byte> userData, ReadOnlySpan<byte> optionalEntropy, bool isLocalMachine = false);
    }
}
