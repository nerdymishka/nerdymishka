using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka
{
    public static class Check
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ArgNotNull<T>([ValidatedNotNull] T value, string parameterName)
        {
            if (value is null)
                throw new ArgumentNullException(parameterName ?? typeof(T).FullName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ArgNotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNotEmptyException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> ArgNotEmpty<T>(ReadOnlySpan<T> value, string parameterName)
        {
            if (value.IsEmpty)
                throw new ArgumentNotEmptyException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<T> ArgNotEmpty<T>(ICollection<T> value, string parameterName)
        {
            Check.ArgNotNull(value, parameterName);

            if (value.Count == 0)
                throw new ArgumentNotEmptyException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ArgNotEmpty<T>(T[] value, string parameterName)
        {
            Check.ArgNotNull(value, parameterName);

            if (value.Length == 0)
                throw new ArgumentNotEmptyException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<T> ArgSize<T>(ICollection<T> value, int expected, string parameterName)
        {
            Check.ArgNotNull(value, parameterName);

            if (value.Count != expected)
                throw new ArgumentOutOfRangeException(
                    parameterName, $"Parameter {parameterName}.Count must be {expected}.");

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<T> ArgMin<T>(ICollection<T> value, int min, string parameterName)
        {
            Check.ArgNotNull(value, parameterName);

            if (value.Count < min)
                throw new ArgumentOutOfRangeException(
                    parameterName, $"Parameter {parameterName}.Count must be at least {min}.");

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ArgNotWhiteSpace(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNotWhiteSpaceException(parameterName);

            return value;
        }
    }
}
