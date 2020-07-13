using System.Collections.Generic;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    public class MemoryProtectedBytesMap
    {
        private Dictionary<int, MemoryProtectedBytes> dictionary =
            new Dictionary<int, MemoryProtectedBytes>();

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

        public void Add(MemoryProtectedBytes bytes)
        {
            if (bytes is null)
                return;

            var index = this.IndexOf(bytes);

            if (index != -1)
                return;

            this.dictionary.Add(this.dictionary.Count, bytes);
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
    }
}