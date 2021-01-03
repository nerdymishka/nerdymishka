using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    public class FlexNull : FlexScalar, IEquatable<FlexNull>
    {
        public static FlexNull Self { get; } = new FlexNull();

        public override bool IsNull => true;

        public override FlexKinds Kind => FlexKinds.Scalar | FlexKinds.Null;

        public bool Equals(FlexNull other) => other is FlexNull;

        public override int GetHashCode()
        {
            return HashCode.Combine(this.IsNull, this.Kind);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is FlexNull other && this.Equals(other);
        }
    }
}
