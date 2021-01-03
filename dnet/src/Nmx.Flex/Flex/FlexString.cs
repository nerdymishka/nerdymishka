using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Flex
{
    public class FlexString : FlexScalar
    {
        private string value = string.Empty;

        public FlexString()
        {
        }

        public FlexString(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value
        {
            get => this.value;
            set => this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override bool IsNull => false;

        public override FlexKinds Kind => FlexKinds.Scalar | FlexKinds.String;

        public void SetGuid(Guid value) => this.value = value.ToString("D");

        public bool TryParseGuid(out Guid value)
            => Guid.TryParseExact(this.value, "D", out value);

        public bool TryParseDateTime(out DateTime value)
            => DateTime.TryParse(this.value.AsSpan(), null, System.Globalization.DateTimeStyles.AssumeUniversal, out value);

        public bool TryParseDateTimeOffset(out DateTimeOffset value)
            => DateTimeOffset.TryParse(this.value.AsSpan(), null, System.Globalization.DateTimeStyles.None, out value);

        public bool TryParseBytes(out byte[] value)
        {
            // we decode string -> byte, so the resulting length will
            // be /4 * 3 - padding. To be on the safe side, keep padding and slice later
            int bufferSize = this.value.Length / 4 * 3;

            byte[] arrayToReturnToPool = null;
            Span<byte> buffer = bufferSize <= 256
                ? stackalloc byte[256]
                : arrayToReturnToPool = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                if (Convert.TryFromBase64String(this.value, buffer, out int bytesWritten))
                {
                    buffer = buffer.Slice(0, bytesWritten);
                    value = buffer.ToArray();
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            finally
            {
                if (arrayToReturnToPool != null)
                {
                    buffer.Clear();
                    ArrayPool<byte>.Shared.Return(arrayToReturnToPool);
                }
            }
        }
    }
}
