using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka.Security.Cryptography
{
    public static class NonceFactory
    {
        private static readonly List<byte[]> Nonces = new List<byte[]>();
        private static object s_syncLock = new object();
        private static NerdyRandomNumberGenerator s_rng = new NerdyRandomNumberGenerator();

        public static void Clear(bool clearArray = false)
        {
            lock (s_syncLock)
            {
                if (clearArray)
                {
                    foreach (var iv in Nonces)
                    {
                        Array.Clear(iv, 0, iv.Length);
                    }
                }

                Nonces.Clear();
            }
        }

        public static bool Remove(byte[] iv)
        {
            lock (s_syncLock)
            {
                var index = Nonces.FindIndex(o => o.SequenceEqual(iv));
                if (index > -1)
                {
                    Nonces.RemoveAt(index);
                    return true;
                }

                return false;
            }
        }

        public static byte[] Generate(int size = 8)
        {
            lock (s_syncLock)
            {
                unsafe
                {
                    var set = new byte[size];
                    s_rng.NextBytes(set);

                    while (Nonces.Any(o => o.SequenceEqual(set)))
                    {
                        s_rng.NextBytes(set);
                    }

                    Nonces.Add(set);

                    return set;
                }
            }
        }
    }
}