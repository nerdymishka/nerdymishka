using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Api.KeePass
{
    public interface IProtectedDataProvider
    {
        byte[] ProtectData(byte[] userData, byte[] optionalEntropy, bool isLocalMachine = false);

        byte[] UnprotectData(byte[] userData, byte[] optionalEntropy, bool isLocalMachine = false);
    }
}
