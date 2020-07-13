using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    public class MemoryProtectedTextDictionary :
        IDictionary<string, MemoryProtectedText>,
        IEquatable<MemoryProtectedTextDictionary>
    {
        private readonly SortedDictionary<string, MemoryProtectedText> dictionary =
            new SortedDictionary<string, MemoryProtectedText>(StringComparer.OrdinalIgnoreCase);

        public ICollection<string> Keys => this.dictionary.Keys;

        public ICollection<MemoryProtectedText> Values => this.dictionary.Values;

        public int Count => this.dictionary.Count;

        public bool IsReadOnly => false;

        public MemoryProtectedText this[string key]
        {
            get
            {
                if (this.dictionary.TryGetValue(key, out MemoryProtectedText value))
                    return value;

                return default;
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        public bool SetValue(string key, ReadOnlySpan<byte> bytes)
        {
            var other = this[key];
            if (other is null)
            {
                other = new MemoryProtectedText(bytes);
                this.Add(key, other);
                return true;
            }

            var byteArray = bytes.ToArray();
            if (!other.Equals(byteArray))
            {
                other.Dispose();
                other = new MemoryProtectedText(bytes);
                this[key] = other;
                Array.Clear(byteArray, 0, byteArray.Length);
                return true;
            }

            Array.Clear(byteArray, 0, byteArray.Length);
            return false;
        }

        public bool SetValue(string key, ReadOnlySpan<char> chars)
        {
            var other = this[key];
            var charArray = chars.ToArray();
            if (other is null)
            {
                other = new MemoryProtectedText(charArray);
                Array.Clear(charArray, 0, charArray.Length);
                this.Add(key, other);
                return true;
            }

            var byteArray = NerdyMishka.Text.Utf8Options.NoBom.GetBytes(charArray);
            if (!other.Equals(byteArray))
            {
                other.Dispose();
                other = new MemoryProtectedText(byteArray);
                this[key] = other;
                Array.Clear(charArray, 0, charArray.Length);
                Array.Clear(byteArray, 0, byteArray.Length);
                return true;
            }

            Array.Clear(charArray, 0, charArray.Length);
            Array.Clear(byteArray, 0, byteArray.Length);
            return false;
        }

        public bool SetValue(string key, SecureString value)
        {
            var other = this[key];
            if (other is null)
            {
                other = new MemoryProtectedText(value);
                this.Add(key, other);
                return true;
            }

            var bytes = value.ToBytes(NerdyMishka.Text.Utf8Options.NoBom);
            if (!other.Equals(bytes))
            {
                other.Dispose();
                other = new MemoryProtectedText(bytes);
                this[key] = other;
                Array.Clear(bytes, 0, bytes.Length);
                return true;
            }

            Array.Clear(bytes, 0, bytes.Length);
            return false;
        }

        public string ReadAsString(string key)
        {
            if (this.dictionary.TryGetValue(key, out MemoryProtectedText mp))
            {
                return mp.ToString(true);
            }

            return null;
        }

        public ReadOnlySpan<char> ReadAsCharSpan(string key)
        {
            if (this.dictionary.TryGetValue(key, out MemoryProtectedText mp))
            {
                return mp.ToCharSpan();
            }

            return Array.Empty<char>();
        }

        public ReadOnlySpan<byte> ReadAsByteSpan(string key)
        {
            if (this.dictionary.TryGetValue(key, out MemoryProtectedText mp))
            {
                return mp.ToByteSpan();
            }

            return Array.Empty<byte>();
        }

        public SecureString ReadAsSecureString(string key)
        {
            if (this.dictionary.TryGetValue(key, out MemoryProtectedText mp))
            {
                return mp.ToSecureString();
            }

            return null;
        }

        public void Add(string key, MemoryProtectedText value)
        {
            this.dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, MemoryProtectedText> item)
        {
            this.dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
            => this.dictionary.Clear();

        public bool Contains(KeyValuePair<string, MemoryProtectedText> item)
        {
            foreach (var item2 in this.dictionary)
            {
                if (item2.Equals(item))
                    return true;
            }

            return false;
        }

        public bool ContainsKey(string key)
            => this.dictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, MemoryProtectedText>[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            int i = arrayIndex;
            int l = Math.Min(this.Count, array.Length - i);
            foreach (var item in this.dictionary)
            {
                array[i] = item;
                i++;
                if (i >= l)
                    break;
            }
        }

        public IEnumerator<KeyValuePair<string, MemoryProtectedText>> GetEnumerator()
            => this.dictionary.GetEnumerator();

        public bool Remove(string key)
            => this.dictionary.Remove(key);

        public bool Remove(KeyValuePair<string, MemoryProtectedText> item)
        {
            var result = this[item.Key];
            if (result is object && result.Equals(item.Value))
                return this.Remove(item.Key);

            return false;
        }

        public bool TryGetValue(string key, out MemoryProtectedText value)
            => this.dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public override int GetHashCode()
            => this.dictionary.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is MemoryProtectedTextDictionary other)
                return this.Equals(other);

            return false;
        }

        public bool Equals(MemoryProtectedTextDictionary other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (this.Count != other.Count)
                return false;

            foreach (var kv in other)
            {
                if (!this.ContainsKey(kv.Key))
                    return false;

                var item = this[kv.Key];
                if (!item.Equals(kv.Value))
                    return false;
            }

            return true;
        }
    }
}