using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NerdyMishka.Util.Strings;

namespace NerdyMishka
{
    public class SymbolFactory<T>
        where T : Symbol<T>, new()
    {
        private readonly ConcurrentDictionary<string, T> lookup =
            new ConcurrentDictionary<string, T>();

        static SymbolFactory()
        {
        }

        public IReadOnlyCollection<T> All => this.lookup.Values.ToList();

        public T Find(string label)
        {
            var key = this.lookup.Keys.SingleOrDefault(o => o.EqualsOrdinal(label));
            if (key == null)
                return default;

            this.lookup.TryGetValue(key, out T value);
            return value;
        }

        public T Create(string label)
        {
            return this.Create(label, new T());
        }

        public T Create(string label, T item, bool intern = false)
        {
            if (this.lookup.ContainsKey(label))
                throw new ArgumentException($"Key {label} already exists");

            if (intern)
                label = string.Intern(label);

            if (item != null)
                item.Name = label;

            this.lookup.TryAdd(label, item);
            return item;
        }
    }
}