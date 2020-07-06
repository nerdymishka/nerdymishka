using System;
using System.Collections.Generic;
using System.Linq;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    internal static class KeePassIdentifierFactory
    {
        private static readonly List<byte[]> Uuids = new List<byte[]>();
        private static readonly object SyncLock = new object();
        private static readonly NerdyRandomNumberGenerator RandomNumberGenerator = new NerdyRandomNumberGenerator();

        public static void Clear(bool clearArray = false)
        {
            lock (SyncLock)
            {
                if (clearArray)
                {
                    foreach (var iv in Uuids)
                    {
                        Array.Clear(iv, 0, iv.Length);
                    }
                }

                Uuids.Clear();
            }
        }

        public static bool Remove(byte[] iv)
        {
            lock (SyncLock)
            {
                var index = Uuids.FindIndex(o => o.SequenceEqual(iv));
                if (index > -1)
                {
                    Uuids.RemoveAt(index);
                    return true;
                }

                return false;
            }
        }

        public static byte[] Generate(int size = 16)
        {
            lock (SyncLock)
            {
                unsafe
                {
                    var set = new byte[size];
                    RandomNumberGenerator.NextBytes(set);

                    while (Uuids.Any(o => o.SequenceEqual(set)))
                    {
                        RandomNumberGenerator.NextBytes(set);
                    }

                    Uuids.Add(set);

                    return set;
                }
            }
        }
    }
}