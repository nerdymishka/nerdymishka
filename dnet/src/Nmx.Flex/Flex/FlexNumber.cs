using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    public class FlexNumber : FlexScalar
    {
        private string value = "0";

        public FlexNumber()
        {
        }

        public FlexNumber(string value) => this.SetFormattedNumber(value);

        public FlexNumber(byte value) => this.SetByte(value);

        public FlexNumber(decimal value) => this.SetDecimal(value);

        public FlexNumber(double value) => this.SetDouble(value);

        public FlexNumber(int value) => this.SetInt32(value);

        public FlexNumber(long value) => this.SetInt64(value);

        public FlexNumber(short value) => this.SetInt16(value);

        public override bool IsNull => false;

        public override FlexKinds Kind => FlexKinds.Scalar | FlexKinds.Number;

        public string Value
        {
            get => this.value;
            set => this.SetFormattedNumber(value);
        }

        public void SetByte(byte value) => this.value = value.ToString(CultureInfo.InvariantCulture);

        public void SetInt16(short value) => this.value = value.ToString(CultureInfo.InvariantCulture);

        public void SetInt32(int value) => this.value = value.ToString(CultureInfo.InvariantCulture);

        public void SetInt64(long value) => this.value = value.ToString(CultureInfo.InvariantCulture);

        public void SetDouble(double value) => this.value = value.ToString(CultureInfo.InvariantCulture);

        public void SetDecimal(decimal value) => this.value = value.ToString(CultureInfo.InvariantCulture);

        public bool TryGetByte(out byte value) => byte.TryParse(this.value, out value);

        public bool TryGetInt16(out short value) => short.TryParse(this.value, out value);

        public bool TryGetInt32(out int value) => int.TryParse(this.value, out value);

        public bool TryGetInt64(out long value) => long.TryParse(this.value, out value);

        public bool TryGetDouble(out double value) => double.TryParse(this.value, out value);

        public bool TryGetDecimal(out decimal value) => decimal.TryParse(this.value, out value);

        public void SetFormattedNumber(string value)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            // TODO: test value
            this.value = value;
        }
    }
}
