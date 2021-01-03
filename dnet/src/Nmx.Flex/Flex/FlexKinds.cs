using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    [Flags]
#pragma warning disable CA1028 // Enum Storage should be Int32
    public enum FlexKinds : short
#pragma warning restore CA1028 // Enum Storage should be Int32
    {
        Map = 1,
        List = 2,
        Scalar = 4,
        Comment = 8,
        Null = 16,
        Boolean = 32,
#pragma warning disable CA1720 // Identifier contains type name
        String = 64,
#pragma warning restore CA1720 // Identifier contains type name
        Number = 128,
        Date = 256,
    }
}
