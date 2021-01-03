using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Collections.Extensions;

namespace NerdyMishka.Flex
{
    public class FlexMap : IFlexMap
    {
        private OrderedDictionary<string, IFlexNode> properties = Util.EmptyMap;

        private OrderedDictionary<string, IFlexScalar> attributes;

        public ICollection<string> Keys => this.properties.Keys;

        public ICollection<IFlexNode> Values => this.properties.Values;

        public int Count => this.properties.Count;

        bool ICollection<KeyValuePair<string, IFlexNode>>.IsReadOnly => false;

        IEnumerable<string> IReadOnlyDictionary<string, IFlexNode>.Keys => ((IReadOnlyDictionary<string, IFlexNode>)this.properties).Keys;

        IEnumerable<IFlexNode> IReadOnlyDictionary<string, IFlexNode>.Values => ((IReadOnlyDictionary<string, IFlexNode>)this.properties).Values;

        FlexKinds IFlexNode.Kind => FlexKinds.Map;

        bool IFlexNode.HasChildren => this.properties.Count > 0;

        bool IFlexNode.HasAttributes => this.attributes != null && this.attributes.Count > 0;

        public OrderedDictionary<string, IFlexScalar> Attributes
        {
            get
            {
                this.attributes ??= new OrderedDictionary<string, IFlexScalar>();
                return this.attributes;
            }
        }

        IFlexComment IFlexNode.Comment { get; set; }

        public IFlexNode this[string key]
        {
            get
            {
                if (this.properties.TryGetValue(key, out IFlexNode element))
                    return element;

                return null;
            }

            set
            {
                this.properties[key] = value;
            }
        }

        public void Add(string key, IFlexNode value)
        {
            if (ReferenceEquals(this.properties, Util.EmptyMap))
                this.properties = new OrderedDictionary<string, IFlexNode>();

            this.properties.Add(key, value);
        }

        public void Insert(int index, string key, IFlexNode value)
        {
            if (ReferenceEquals(this.properties, Util.EmptyMap))
                this.properties = new OrderedDictionary<string, IFlexNode>();

            this.properties.Insert(index, key, value);
        }

        void ICollection<KeyValuePair<string, IFlexNode>>.Add(KeyValuePair<string, IFlexNode> item)
           => ((ICollection<KeyValuePair<string, IFlexNode>>)this.properties).Add(item);

        public void Clear()
            => this.properties.Clear();

        bool ICollection<KeyValuePair<string, IFlexNode>>.Contains(KeyValuePair<string, IFlexNode> item)
            => ((ICollection<KeyValuePair<string, IFlexNode>>)this.properties).Contains(item);

        public bool ContainsKey(string key)
            => this.properties.ContainsKey(key);

        void ICollection<KeyValuePair<string, IFlexNode>>.CopyTo(KeyValuePair<string, IFlexNode>[] array, int arrayIndex)
            => ((ICollection<KeyValuePair<string, IFlexNode>>)this.properties).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, IFlexNode>> GetEnumerator()
            => this.properties.GetEnumerator();

        public void RemoveAt(int index)
            => this.properties.RemoveAt(index);

        public bool Remove(string key)
            => this.properties.Remove(key);

        bool ICollection<KeyValuePair<string, IFlexNode>>.Remove(KeyValuePair<string, IFlexNode> item)
            => ((ICollection<KeyValuePair<string, IFlexNode>>)this.properties).Remove(item);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out IFlexNode value)
            => this.properties.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        IFlexNode IFlexNode.GetChild(string path)
        {
            var segments = path.Split('/');
            var target = (IFlexNode)this;
            foreach (var segment in segments)
            {
                var next = target[segment];
                if (next is null)
                {
                    return null;
                }

                target = next;
            }

            return target;
        }

        IEnumerator<IFlexNode> IEnumerable<IFlexNode>.GetEnumerator()
            => this.Values.GetEnumerator();
    }
}