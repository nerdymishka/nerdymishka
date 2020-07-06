using System;
using System.Runtime.InteropServices;
using System.Security;
using NerdyMishka.Text;
using NerdyMishka.Util.Arrays;

namespace NerdyMishka.Api.KeePass.Package
{
    public class KeePassPasswordFragment : KeePassCompositeKeyFragment
    {
        public KeePassPasswordFragment(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            var encoding = Utf8Options.NoBom;
            IntPtr bstr = IntPtr.Zero;
            char[] charArray = new char[secureString.Length];

            try
            {
                bstr = Marshal.SecureStringToBSTR(secureString);
                Marshal.Copy(bstr, charArray, 0, charArray.Length);

                ReadOnlySpan<byte> bytes = encoding.GetBytes(charArray);
                charArray.Clear();

                this.SetData(bytes.ToSHA256Hash());
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        public KeePassPasswordFragment(ReadOnlySpan<byte> password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (password.Length == 0)
                throw new ArgumentException("password must be greater than 0 characters");

            var bytes = password.ToSHA256Hash();
            this.SetData(bytes);
        }

        public KeePassPasswordFragment(ReadOnlySpan<char> password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (password.Length == 0)
                throw new ArgumentException("password must be greater than 0 characters");

            var chars = password.ToArray();
            ReadOnlySpan<byte> pw = System.Text.Encoding.UTF8
                .GetBytes(chars);

            Array.Clear(chars, 0, chars.Length);

            this.SetData(pw.ToSHA256Hash());
        }
    }
}