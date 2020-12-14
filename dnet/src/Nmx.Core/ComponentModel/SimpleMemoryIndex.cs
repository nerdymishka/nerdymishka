using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.ComponentModel
{
    public class SimpleMemoryIndex<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> index;

        public SimpleMemoryIndex()
            : this(EqualityComparer<TKey>.Default)
        {
        }

        public SimpleMemoryIndex(IEqualityComparer<TKey> equalityComparer)
        {
            Check.ArgNotNull(equalityComparer, nameof(equalityComparer));

            this.index = new Dictionary<TKey, TValue>(equalityComparer);
        }

        public SimpleMemoryIndex(IDictionary<TKey, TValue> values)
            : this(values, EqualityComparer<TKey>.Default)
        {
        }

        public SimpleMemoryIndex(IDictionary<TKey, TValue> values, IEqualityComparer<TKey> equalityComparer)
        {
            Check.ArgNotNull(values, nameof(values));
            Check.ArgNotNull(equalityComparer, nameof(equalityComparer));

            this.index = new Dictionary<TKey, TValue>(equalityComparer);
            foreach (var key in values.Keys)
                this.Add(key, values[key]);
        }

        public int Count => this.index.Count;

        public TValue Find(TKey key)
        {
            return this.index.TryGetValue(key, out var value) ? value : default;
        }

        public void Add(TKey key, TValue value) => this.index.Add(key, value);

        public void Remove(TKey key) => this.index.Remove(key);

        public void Clear() => this.index.Clear();
    }
}
