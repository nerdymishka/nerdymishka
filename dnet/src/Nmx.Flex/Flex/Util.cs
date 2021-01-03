using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Collections.Extensions;

namespace NerdyMishka.Flex
{
    internal static class Util
    {
        public static OrderedDictionary<string, IFlexNode> EmptyMap { get; } = new OrderedDictionary<string, IFlexNode>(0);

        public static OrderedDictionary<string, IFlexScalar> EmptyAttributes { get; } = new OrderedDictionary<string, IFlexScalar>(0);

        public static List<IFlexNode> EmptyList { get; } = new List<IFlexNode>();

        internal static UTF8Encoding Utf8 { get; } = new UTF8Encoding(false, true);
    }
}
