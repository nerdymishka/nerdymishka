using System;
using System.Collections.Generic;

namespace NerdyMishka.Models
{
    public class StringModelIndex<TValue> : ModelIndex<string, TValue>
    {
        public StringModelIndex(IDictionary<string, TValue> values)
            : base(values, new StringEquality())
        {
        }

        internal class StringEquality : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                if (x is null && y is null)
                    return true;

                if (x != null)
                {
                    return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }

            public int GetHashCode(string obj)
            {
                if (obj is null)
                    return 0;

                return obj.GetHashCode();
            }
        }
    }
}