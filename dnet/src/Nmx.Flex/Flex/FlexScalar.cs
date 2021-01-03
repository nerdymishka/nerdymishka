using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Collections.Extensions;

namespace NerdyMishka.Flex
{
    public abstract class FlexScalar : IFlexScalar
    {
        private OrderedDictionary<string, IFlexScalar> attributes;

        public abstract bool IsNull { get; }

        public abstract FlexKinds Kind { get; }

        bool IFlexNode.HasChildren => false;

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

        IFlexNode IFlexNode.this[string key]
        {
            get => default;
            set => throw new NotImplementedException();
        }

        IFlexNode IFlexNode.GetChild(string path)
            => default;

        IEnumerator<IFlexNode> IEnumerable<IFlexNode>.GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }
    }
}