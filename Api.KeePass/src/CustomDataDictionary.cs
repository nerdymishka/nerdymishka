using System;
using System.Collections;
using System.Collections.Generic;

namespace NerdyMishka.Api.KeePass
{
    public class CustomDataDictionary :
        IDictionary<string, string>,
        IEquatable<CustomDataDictionary>
    {
        private SortedDictionary<string, string> dictionary =
            new SortedDictionary<string, string>(StringComparer.Ordinal);

        public ICollection<string> Keys
            => this.dictionary.Keys;

        public ICollection<string> Values
            => this.dictionary.Values;

        public int Count => this.dictionary.Count;

        public bool IsReadOnly => false;

        public string this[string key]
        {
            get
            {
                if (this.dictionary.TryGetValue(key, out string value))
                    return value;

                return default;
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        public void Add(string key, string value)
            => this.Add(key, value);

        public void Add(KeyValuePair<string, string> item)
        {
            if (!this.ContainsKey(item.Key))
                this.Add(item.Key, item.Value);
        }

        public void Clear()
            => this.dictionary.Clear();

        public bool Contains(KeyValuePair<string, string> item)
        {
            var other = this[item.Key];
            if (ReferenceEquals(other, item.Value) ||
                other.Equals(item))
                return true;

            return false;
        }

        public bool ContainsKey(string key)
            => this.dictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
            => this.dictionary.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is CustomDataDictionary other)
                return this.Equals(other);

            return false;
        }

        public bool Equals(CustomDataDictionary other)
        {
            if (other is object)
            {
                if (ReferenceEquals(this, other))
                    return true;

                if (this.Count != other.Count)
                    return false;

                foreach (var kv in other)
                {
                    if (!this.ContainsKey(kv.Key))
                        return false;

                    var item = this[kv.Key];
                    bool equal = ReferenceEquals(item, kv.Value) ||
                        item.Equals(kv.Value, StringComparison.Ordinal);
                    if (!equal)
                        return false;
                }

                return true;
            }

            return false;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => this.dictionary.GetEnumerator();

        public bool Remove(string key)
            => this.dictionary.Remove(key);

        public bool Remove(KeyValuePair<string, string> item)
        {
            var other = this[item.Key];
            if (ReferenceEquals(other, item.Value) ||
                other.Equals(item.Value, StringComparison.Ordinal))
            {
                return this.dictionary.Remove(item.Key);
            }

            return false;
        }

        public bool TryGetValue(string key, out string value)
            => this.dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}