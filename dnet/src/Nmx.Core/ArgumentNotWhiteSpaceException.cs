using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka
{
    public class ArgumentNotWhiteSpaceException : ArgumentNullException
    {
        public ArgumentNotWhiteSpaceException()
        {
        }

        public ArgumentNotWhiteSpaceException(string parameterName)
            : base(parameterName)
        {
        }

        public ArgumentNotWhiteSpaceException(string parameterName, string message)
            : base(parameterName, message)
        {
        }

        public ArgumentNotWhiteSpaceException(string parameterName, Exception innerException)
            : base(parameterName, innerException)
        {
        }

        protected ArgumentNotWhiteSpaceException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
