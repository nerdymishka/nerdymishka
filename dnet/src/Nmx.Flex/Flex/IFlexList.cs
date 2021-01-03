using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    public interface IFlexList : IFlexNode,
        IList<IFlexNode>,
        ICollection<IFlexNode>,
        IEnumerable<IFlexNode>,
        IReadOnlyCollection<IFlexNode>,
        IReadOnlyList<IFlexNode>
    {
    }
}
