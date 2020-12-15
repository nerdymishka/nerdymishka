using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using NerdyMishka.Text;

namespace NerdyMishka.Security.Cryptography
{
    public static class EncryptionUtil
    {
        public static ReadOnlySpan<byte> ComputeChecksum(
            Stream stream,
            HashAlgorithmName name,
            bool resetStream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            using (var algo = Create(name))
            {
                var result = algo.ComputeHash(stream);

                if (resetStream && stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                return result;
            }
        }

        public static ReadOnlySpan<byte> ComputeChecksum(
            ReadOnlySpan<byte> bytes,
            HashAlgorithmName name)
        {
            using (var algo = Create(name))
            {
#if NETSTANDARD2_0
                var array = bytes.ToArray();
                var result = algo.ComputeHash(array);
                Array.Clear(array, 0, array.Length);
                return result;
#else
                var size = algo.HashSize >> 3;
                Span<byte> uiSpan = stackalloc byte[64];
                uiSpan = uiSpan.Slice(0, size);
                if (!algo.TryComputeHash(bytes, uiSpan, out int bytesWritten) || bytesWritten != size)
                {
                    throw new CryptographicException();
                }

                return uiSpan.ToArray();
#endif
            }
        }

        public static ReadOnlyMemory<byte> ComputeChecksumAsMemory(
            ReadOnlyMemory<byte> bytes,
            HashAlgorithmName name)
        {
            if (!MemoryMarshal.TryGetArray(bytes, out ArraySegment<byte> segment))
            {
                throw new CryptographicException("Could not retrieve array segment from memory");
            }

            using (var algo = Create(name))
            {
#if NETSTANDARD2_0
                return algo.ComputeHash(segment.Array);
#else

                var size = algo.HashSize >> 3;
                Span<byte> uiSpan = stackalloc byte[64];
                uiSpan = uiSpan.Slice(0, size);
                if (!algo.TryComputeHash(segment.Array, uiSpan, out int bytesWritten) || bytesWritten != size)
                {
                    throw new CryptographicException();
                }

                return uiSpan.ToArray();
#endif
            }
        }

        public static HashAlgorithm Create(HashAlgorithmName hashAlgorithm)
        {
            return HashAlgorithm.Create(hashAlgorithm.Name);
        }

        public static bool SlowEquals(IList<byte> left, IList<byte> right)
        {
            if (left == null)
                return false;

            if (right == null)
                return false;

            uint diff = (uint)left.Count ^ (uint)left.Count;
            for (int i = 0; i < left.Count; i++)
            {
                diff |= (uint)(left[i] ^ right[i]);
            }

            return diff == 0;
        }

        public static byte[] CreateOutputBuffer(byte[] inputBuffer, int blockSize)
        {
            Check.ArgNotNull(inputBuffer, nameof(inputBuffer));

            var l = inputBuffer.Length;
            var actualBlockSize = blockSize / 8;
            var pad = l % actualBlockSize;
            if (pad != 0)
            {
                return new byte[l + (actualBlockSize - pad)];
            }

            return new byte[l];
        }

        public static byte[] ToBytes(this SecureString secureString, Encoding encoding = null)
        {
            Check.ArgNotNull(secureString, nameof(secureString));

            IntPtr bstr = IntPtr.Zero;
            var charArray = new char[secureString.Length];
            encoding = encoding ?? Utf8Options.NoBom;

            try
            {
                bstr = Marshal.SecureStringToBSTR(secureString);
                Marshal.Copy(bstr, charArray, 0, charArray.Length);

                var bytes = encoding.GetBytes(charArray);
                return bytes;
            }
            finally
            {
                if (charArray != null && charArray.Length > 0)
                    Array.Clear(charArray, 0, charArray.Length);

                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        /*
             public static byte[] ToBytes(this SecureString secureString, Encoding encoding = null)
             {
                 Check.NotNull(nameof(secureString), secureString);

                 IntPtr bstr = IntPtr.Zero;
                 char[] charArray = new char[secureString.Length];
                 encoding = encoding ?? NerdyMishka.Text.Utf8.NoBomNoThrow;

                 try
                 {
                     bstr = Marshal.SecureStringToBSTR(secureString);
                     Marshal.Copy(bstr, charArray, 0, charArray.Length);

                     var bytes = encoding.GetBytes(charArray);
                     charArray.Clear();
                     return bytes;
                 }
                 finally
                 {
                     Marshal.ZeroFreeBSTR(bstr);
                 }
             } */
    }
}
