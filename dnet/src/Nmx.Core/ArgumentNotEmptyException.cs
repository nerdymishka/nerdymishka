using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka
{
    public class ArgumentNotEmptyException : ArgumentNullException
    {
        public ArgumentNotEmptyException()
        {
        }

        public ArgumentNotEmptyException(string parameterName)
            : base(parameterName)
        {
        }

        public ArgumentNotEmptyException(string parameterName, string message)
            : base(parameterName, message)
        {
        }

        public ArgumentNotEmptyException(string parameterName, Exception innerException)
            : base(parameterName, innerException)
        {
        }

        protected ArgumentNotEmptyException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
