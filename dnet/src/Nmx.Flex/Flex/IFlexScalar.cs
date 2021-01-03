using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    public interface IFlexScalar : IFlexNode
    {
        bool IsNull { get; }
    }
}
