using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Collections.Extensions;

namespace NerdyMishka.Flex
{
    public interface IFlexNode : IEnumerable<IFlexNode>
    {
        FlexKinds Kind { get; }

        bool HasChildren { get; }

        bool HasAttributes { get; }

        OrderedDictionary<string, IFlexScalar> Attributes { get; }

        IFlexComment Comment { get; set; }

        IFlexNode this[string key] { get; set; }

        IFlexNode GetChild(string path);
    }
}
