using System.Collections;
using System.Collections.Generic;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    public class MemoryProtectedBytesMap :
        IEnumerable<KeyValuePair<int, MemoryProtectedBytes>>
    {
        private Dictionary<int, MemoryProtectedBytes> dictionary =
            new Dictionary<int, MemoryProtectedBytes>();

        private int nextIndentity = 0;

        public int Count => this.dictionary.Count;

        public MemoryProtectedBytes this[int key]
        {
            get
            {
                if (key < 0 && key >= this.dictionary.Count)
                    return default;

                if (this.dictionary.TryGetValue(key, out MemoryProtectedBytes bytes))
                    return bytes;

                return default;
            }

            set
            {
                this[key] = value;
            }
        }

        public void Add(int key, MemoryProtectedBytes bytes)
        {
            this.dictionary.Add(key, bytes);
        }

        public void Add(MemoryProtectedBytes bytes)
        {
            if (bytes is null)
                return;

            var index = this.IndexOf(bytes);

            if (index != -1)
                return;

            while (this.dictionary.ContainsKey(index))
            {
                this.nextIndentity++;
                index = this.nextIndentity;
            }

            this.dictionary.Add(index, bytes);
            this.nextIndentity++;
        }

        public void Add(IKeePassGroup group)
        {
            if (group is null)
                return;

            if (group.Entries.Count != 0)
            {
                foreach (var entry in group.Entries)
                {
                    this.Add(entry);
                }
            }

            if (group.Groups.Count == 0)
                return;

            foreach (var child in group.Groups)
            {
                this.Add(child);
            }
        }

        public void Add(IKeePassEntry entry)
        {
            if (entry == null)
                return;

            this.Add(entry.Binaries);
            if (entry.History.Count == 0)
                return;

            foreach (var historyEntry in entry.History)
            {
                this.Add(historyEntry.Binaries);
            }
        }

        public void Add(MemoryProtectedBytesDictionary bytesDictionary)
        {
            if (bytesDictionary is null)
                return;

            if (bytesDictionary.Count == 0)
                return;

            foreach (var pair in bytesDictionary)
                this.Add(pair.Value);
        }

        public void Clear()
            => this.dictionary.Clear();

        public int IndexOf(MemoryProtectedBytes bytes)
        {
            if (bytes is null)
                return -1;

            foreach (var pair in this.dictionary)
            {
                if (ReferenceEquals(pair.Value, bytes) ||
                    bytes.Equals(pair.Value))
                {
                    return pair.Key;
                }
            }

            return -1;
        }

        public IEnumerator<KeyValuePair<int, MemoryProtectedBytes>> GetEnumerator()
            => this.dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}