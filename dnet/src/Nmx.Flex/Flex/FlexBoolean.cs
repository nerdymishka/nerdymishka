using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    public class FlexBoolean : FlexScalar
    {
        private bool isReadOnly;
        private bool value;

        public FlexBoolean()
        {
        }

        public FlexBoolean(bool value) => this.Value = value;

        public FlexBoolean(bool value, bool isReadOnly)
            : this(value)
            => this.isReadOnly = isReadOnly;

        public static FlexBoolean True { get; } = new FlexBoolean(true, true);

        public static FlexBoolean False { get; } = new FlexBoolean(false, true);

        public bool Value
        {
            get => this.value;
            set
            {
                if (!this.isReadOnly)
                    this.value = value;
            }
        }

        public override bool IsNull => false;

        public override FlexKinds Kind => FlexKinds.Scalar | FlexKinds.Boolean;
    }
}
