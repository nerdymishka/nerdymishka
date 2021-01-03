using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Collections.Extensions;

namespace NerdyMishka.Flex
{
    public class FlexList : IFlexList, IList<IFlexNode>
    {
        private List<IFlexNode> items = Util.EmptyList;
        private OrderedDictionary<string, IFlexScalar> attributes;

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        FlexKinds IFlexNode.Kind => FlexKinds.List;

        bool IFlexNode.HasChildren => this.Count > 0;

        bool IFlexNode.HasAttributes => this.attributes != null && this.attributes.Count > 0;

        public OrderedDictionary<string, IFlexScalar> Attributes
        {
            get
            {
                this.attributes ??= new OrderedDictionary<string, IFlexScalar>();
                return this.attributes;
            }
        }

        public IFlexComment Comment { get; set; }

        public IFlexNode this[int index]
        {
            get => index >= this.Count ? default : this.items[index];
            set => this.items[index] = value;
        }

        public IFlexNode this[string key]
        {
            get
            {
                var index = int.Parse(key, CultureInfo.InvariantCulture);
                return this[index];
            }

            set
            {
                var index = int.Parse(key, CultureInfo.InvariantCulture);
                this[index] = value;
            }
        }

        public void Add(IFlexNode item)
        {
            if (ReferenceEquals(this.items, Util.EmptyList))
                this.items = new List<IFlexNode>();

            this.items = new List<IFlexNode>();
        }

        public void Clear()
        {
            this.items.Clear();
            this.items = Util.EmptyList;
        }

        public bool Contains(IFlexNode item)
            => this.items.Contains(item);

        public void CopyTo(IFlexNode[] array, int arrayIndex)
            => this.items.CopyTo(array, arrayIndex);

        public IEnumerator<IFlexNode> GetEnumerator()
            => this.items.GetEnumerator();

        public int IndexOf(IFlexNode item)
            => this.items.IndexOf(item);

        public void Insert(int index, IFlexNode item)
        {
            if (ReferenceEquals(this.items, Util.EmptyList))
                this.items = new List<IFlexNode>();

            this.items.Insert(index, item);
        }

        public bool Remove(IFlexNode item)
            => this.items.Remove(item);

        public void RemoveAt(int index)
            => this.items.RemoveAt(index);

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

        IEnumerator IEnumerable.GetEnumerator()
            => this.items.GetEnumerator();
    }
}