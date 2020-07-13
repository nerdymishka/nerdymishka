using System;
using System.Collections;
using System.Collections.Generic;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    public class MemoryProtectedBytesDictionary :
        IDictionary<string, MemoryProtectedBytes>,
        IEquatable<MemoryProtectedBytesDictionary>
    {
        private readonly SortedDictionary<string, MemoryProtectedBytes> dictionary
            = new SortedDictionary<string, MemoryProtectedBytes>();

        public ICollection<string> Keys => this.dictionary.Keys;

        public ICollection<MemoryProtectedBytes> Values => this.dictionary.Values;

        public int Count => this.dictionary.Count;

        public bool IsReadOnly => false;

        public MemoryProtectedBytes this[string key]
        {
            get
            {
                if (this.dictionary.TryGetValue(key, out MemoryProtectedBytes bytes))
                {
                    return bytes;
                }

                return default;
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        public void Add(string key, MemoryProtectedBytes value)
        {
            this.dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<string, MemoryProtectedBytes> item)
            => this.Add(item.Key, item.Value);

        public void Clear()
            => this.dictionary.Clear();

        public bool Contains(KeyValuePair<string, MemoryProtectedBytes> item)
        {
            var other = this[item.Key];
            if (other is object)
            {
                return other.Equals(item);
            }

            return false;
        }

        public bool ContainsKey(string key)
            => this.dictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, MemoryProtectedBytes>[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            int i = arrayIndex,
                l = Math.Min(this.Count, array.Length - (i + 1));

            foreach (var item in this.dictionary)
            {
                array[i] = item;
                i++;
                if (i >= l)
                    break;
            }
        }

        public IEnumerator<KeyValuePair<string, MemoryProtectedBytes>> GetEnumerator()
            => this.dictionary.GetEnumerator();

        public bool Remove(string key)
            => this.dictionary.Remove(key);

        public bool Remove(KeyValuePair<string, MemoryProtectedBytes> item)
        {
            var other = this[item.Key];
            if (other.Equals(item))
            {
                return this.Remove(item.Key);
            }

            return false;
        }

        public bool TryGetValue(string key, out MemoryProtectedBytes value)
            => this.dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public override int GetHashCode()
            => this.dictionary.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is MemoryProtectedBytesDictionary other)
                return this.Equals(other);

            return false;
        }

        public bool Equals(MemoryProtectedBytesDictionary other)
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