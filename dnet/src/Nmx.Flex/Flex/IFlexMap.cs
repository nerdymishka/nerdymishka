using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    public interface IFlexMap : IFlexNode,
        IDictionary<string, IFlexNode>,
        IReadOnlyDictionary<string, IFlexNode>,
        ICollection<KeyValuePair<string, IFlexNode>>,
        IReadOnlyCollection<KeyValuePair<string, IFlexNode>>
    {
    }
}
