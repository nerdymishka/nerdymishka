using System;
using System.Security;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassPassword
    {
        void SetValue(SecureString value);

        void SetValue(ReadOnlySpan<char> value);

        void SetValue(ReadOnlySpan<byte> value);

        byte[] ToArray();

        char[] ToCharArray();

        ReadOnlySpan<char> ToReadOnlyCharSpan();

        ReadOnlySpan<byte> ToReadOnlyByteSpan();

        SecureString ToSecureString();

        string ToDecryptedString();
    }
}