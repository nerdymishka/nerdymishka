using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Collections.Extensions;

namespace NerdyMishka.Flex
{
    public class FlexComment : IFlexComment
    {
        public string Value { get; set; }

        public FlexKinds Kind => FlexKinds.Comment;

        public bool HasChildren => false;

        public bool HasAttributes => false;

        public OrderedDictionary<string, IFlexScalar> Attributes => Util.EmptyAttributes;

        public IFlexComment Comment
        {
            get => default;
            set => throw new NotImplementedException();
        }

        public IFlexNode this[string key]
        {
            get => default;
            set => throw new NotImplementedException();
        }

        public IFlexNode GetChild(string path)
            => null;

        public IEnumerator<IFlexNode> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}