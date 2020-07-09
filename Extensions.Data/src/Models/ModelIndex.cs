using System;
using System.Collections.Generic;

namespace NerdyMishka.Models
{
    public class ModelIndex<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> index;

        public ModelIndex(IDictionary<TKey, TValue> values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            foreach (var key in values.Keys)
                this.Add(key, values[key]);
        }

        public ModelIndex(IDictionary<TKey, TValue> values, IEqualityComparer<TKey> equalityComparer)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            this.index = new Dictionary<TKey, TValue>(equalityComparer);
            foreach (var key in values.Keys)
                this.Add(key, values[key]);
        }

        public TValue Find(TKey key)
        {
            if (this.index.TryGetValue(key, out TValue value))
                return value;

            return default;
        }

        public void Add(TKey key, TValue value)
        {
            this.index.Add(key, value);
        }
    }
}