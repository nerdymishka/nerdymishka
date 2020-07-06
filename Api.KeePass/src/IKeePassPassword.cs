using System;
using System.Security;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassPassword
    {
        void SetValue(byte[] decryptedBytes);

        void SetValue(string value);

        void SetValue(char[] value);

        void SetValue(ReadOnlySpan<char> value);

        void SetValue(ReadOnlySpan<byte> value);

        byte[] ToArray();

        char[] ToCharArray();

        ReadOnlySpan<char> ToChars();

        ReadOnlySpan<byte> ToReadOnlySpan();

        SecureString ToSecureString();

        string ToDecryptedString();
    }
}