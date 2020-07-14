using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Api.KeePass.Package
{
    internal static class KeePassExtensionMethods
    {
        public static string ToHexString(this ReadOnlySpan<byte> bytes)
        {
            char[] chars = new char[bytes.Length * 2];
            int bit;
            for (int i = 0; i < bytes.Length; i++)
            {
                bit = bytes[i] >> 4;
                chars[i * 2] = (char)(55 + bit + (((bit - 10) >> 31) & -7));
                bit = bytes[i] & 0xF;
                chars[(i * 2) + 1] = (char)(55 + bit + (((bit - 10) >> 31) & -7));
            }

            return new string(chars);
        }

        public static ReadOnlySpan<byte> ToReadOnlySpan(this IReadOnlyList<byte> list)
        {
            var bytes = new byte[list.Count];
            for (var i = 0; i < list.Count; i++)
                bytes[i] = list[i];

            return bytes;
        }

        public static void WriteHeader(this Stream stream, HeaderField field, KeePassIdentifier data)
        {
            stream.WriteByte((byte)field);
            var span = data.ToReadOnlySpan();
            ushort size = (ushort)span.Length;
            stream.Write(BitConverter.GetBytes(size));
            stream.Write(span);
        }

        public static void WriteHeader(this Stream stream, HeaderField field, ReadOnlyMemory<byte> data)
        {
            WriteHeader(stream, field, data.Span);
        }

        public static void WriteHeader(this Stream stream, HeaderField field, ReadOnlySpan<byte> span)
        {
            stream.WriteByte((byte)field);
            ushort size = (ushort)span.Length;
            stream.Write(BitConverter.GetBytes(size));
            stream.Write(span);
        }

        public static void WriteHeader(this Stream stream, HeaderField field, IReadOnlyList<byte> data)
        {
            WriteHeader(stream, field, data.ToReadOnlySpan());
        }

        public static void WriteHeader(this Stream stream, HeaderField field, byte[] data)
        {
            stream.WriteByte((byte)field);
            ushort size = (ushort)data.Length;
            stream.Write(BitConverter.GetBytes(size));
            stream.Write(data);
        }

        public static long ToInt64(this byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }

        public static int ToInt32(this byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        internal static ushort ToUShort(this byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }

        internal static uint ToUInt(this byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        internal static ReadOnlySpan<byte> ToSHA256Hash(this ReadOnlySpan<byte> bytes)
        {
            using (var hash = SHA256.Create())
            {
                byte[] result = null;
#if NETSTANDARD2_0
                result = hash.ComputeHash(bytes.ToArray());
#else
                var size = hash.HashSize >> 3;
                Span<byte> uiSpan = stackalloc byte[64];
                uiSpan = uiSpan.Slice(0, size);

                if (!hash.TryComputeHash(bytes, uiSpan, out int bytesWritten) || bytesWritten != size)
                {
                    throw new CryptographicException();
                }

                result = uiSpan.ToArray();
#endif
                return result;
            }
        }

        internal static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] results = new byte[count];
            int offset = 0;
            while (count > 0)
            {
                int bitsRead = stream.Read(results, offset, count);
                if (bitsRead == 0)
                    break;

                offset += bitsRead;
                count -= bitsRead;
            }

            if (offset != results.Length)
            {
                byte[] part = new byte[offset];
                Array.Copy(results, part, offset);
                return part;
            }

            return results;
        }

        internal static void Write(this Stream stream, uint value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

#if NETSTANDARD2_0
        internal static void Write(this Stream stream, ReadOnlySpan<byte> span)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(span.Length);
            span.CopyTo(bytes);
            stream.Write(bytes, 0, span.Length);
            ArrayPool<byte>.Shared.Return(bytes, true);
        }
#endif
    }
}