using System;
using System.Security;
using System.Text;
using NerdyMishka.Text;
using NerdyMishka.Util.Collections;

namespace NerdyMishka.Security.Cryptography
{
    public class MemoryProtectedText : MemoryProtectedBytes,
        IEquatable<MemoryProtectedText>,
        IComparable<MemoryProtectedText>
    {
        private string text;
        private int hashCode;
        private bool isDisposed;
        private Encoding encoding;

        public MemoryProtectedText()
            : base()
        {
            this.Length = 0;
        }

        public MemoryProtectedText(SecureString secureString, Encoding encoding = null)
        {
            Check.NotNull(nameof(secureString), secureString);
            this.encoding = encoding ?? Utf8Options.NoBom;
            var bytes = secureString.ToBytes(this.encoding);
            this.Init(bytes, false);
            this.Length = secureString.Length;
        }

        public MemoryProtectedText(string value, Encoding encoding = null)
        {
            Check.NotNull(nameof(value), value);

            this.text = value;
            this.encoding = encoding ?? Utf8Options.NoBom;
            var bytes = this.encoding.GetBytes(value);
            this.Init(bytes, false);
            this.Length = value.Length;
        }

        public MemoryProtectedText(char[] chars, Encoding encoding = null, bool encrypt = true)
        {
            Check.NotNull(nameof(chars), chars);
            this.encoding = encoding ?? Utf8Options.NoBom;
            var bytes = this.encoding.GetBytes(chars);
            this.Init(bytes, encrypt);
            this.Length = chars.Length;
        }

        public MemoryProtectedText(byte[] bytes, Encoding encoding = null, bool encrypt = true)
            : base(bytes, encrypt)
        {
            this.encoding = encoding ?? Utf8Options.NoBom;
        }

        public Encoding Encoding => this.encoding;

        public static bool operator ==(MemoryProtectedText left, MemoryProtectedText right)
        {
            if (left == null)
            {
                if (right == null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(MemoryProtectedText left, MemoryProtectedText right)
        {
            if (left == null)
            {
                if (right != null)
                    return true;

                return false;
            }

            return !left.Equals(right);
        }

        public static bool operator >(MemoryProtectedText left, MemoryProtectedText right)
        {
            if (left == null)
            {
                return false;
            }

            return left.CompareTo(right) == 1;
        }

        public static bool operator <(MemoryProtectedText left, MemoryProtectedText right)
        {
            if (left == null)
            {
                return false;
            }

            return left.CompareTo(right) == -1;
        }

        public static bool operator >=(MemoryProtectedText left, MemoryProtectedText right)
        {
            if (left == null)
            {
                if (left == null)
                    return true;

                return false;
            }

            return left.CompareTo(right) != -1;
        }

        public static bool operator <=(MemoryProtectedText left, MemoryProtectedText right)
        {
            if (left == null)
            {
                if (right == null)
                    return true;

                return false;
            }

            return left.CompareTo(right) != 1;
        }

        public override int GetHashCode()
        {
            if (this.hashCode != 0)
                return this.hashCode;

            this.hashCode = this.Hash.GetHashCode() * 13;
            return this.hashCode;
        }

        public string ToString(bool unprotect = false, Encoding encoding = null)
        {
            this.CheckDisposed();

            if (!unprotect)
                return this.ToString();

            if (this.text != null)
                return this.text;

            var decrypted = this.Decrypt();
            try
            {
                encoding = encoding ?? this.GetEncoding();
                this.encoding = encoding;
                this.text = encoding.GetString(decrypted);
                this.IsProtected = false;
                return this.text;
            }
            finally
            {
                Array.Clear(decrypted, 0, decrypted.Length);
            }
        }

        public override string ToString()
        {
            return "********************";
        }

        public SecureString ToSecureString()
        {
            this.CheckDisposed();

            var set = new char[this.Length];
            this.CopyTo(set);
            var ss = new SecureString();
            foreach (var c in set)
                ss.AppendChar(c);

            Array.Clear(set, 0, set.Length);

            return ss;
        }

        public void CopyTo(char[] array)
        {
            Check.NotNull(nameof(array), array);
            this.CheckDisposed();

            if (this.text != null)
            {
                var l = Math.Min(this.Length, array.Length);
                this.text.CopyTo(0, array, 0, l);
                return;
            }

            var decrypted = this.Decrypt();
            var chars = this.GetEncoding().GetChars(decrypted);
            chars.CopyTo(array, 0);
            Array.Clear(decrypted, 0, decrypted.Length);
            Array.Clear(chars, 0, chars.Length);
        }

        public int CompareTo(MemoryProtectedText other)
        {
            this.CheckDisposed();

            if (other == null)
                return 1;

            int c = this.Length.CompareTo(other.Length);
            if (c != 0)
                return c;

            if (this.text != null && other.text != null)
                return string.CompareOrdinal(this.text, other.text);

            // use hashes to avoid decrypting when possible.
            if (this.Hash.EqualTo(this.Hash))
                return 0;

            var a = new char[this.Length];
            var b = new char[this.Length];
            this.CopyTo(a);
            other.CopyTo(b);

            try
            {
                for (var i = 0; i < this.Length; i++)
                {
                    char l = a[i];
                    char r = b[i];
                    if (l == r)
                        continue;

                    if (l > r)
                        return 1;

                    return -1;
                }

                return 0;
            }
            finally
            {
                Array.Clear(a, 0, a.Length);
                Array.Clear(b, 0, a.Length);
            }
        }

        public override bool Equals(object other)
        {
            this.CheckDisposed();

            if (other == null)
                return false;

            if (other is MemoryProtectedText)
                return this.Equals((MemoryProtectedText)other);

            return false;
        }

        public bool Equals(MemoryProtectedText other)
        {
            this.CheckDisposed();

            if (other == null)
                return false;

            if (this.text != null && other.text != null)
                return this.text == other.text;

            return this.Equals((MemoryProtectedBytes)other);
        }

        protected override void CheckDisposed()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException($"{nameof(MemoryProtectedText)} - {this.IV}");
        }

        protected override void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                this.text = null;
                this.encoding = null;
            }

            this.isDisposed = true;
            base.Dispose(disposing);
        }

        private Encoding GetEncoding() => this.encoding ?? Utf8Options.NoBom;
    }
}