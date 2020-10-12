using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NerdyMishka.Util.Strings;

namespace NerdyMishka.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> EnumerateTypeInheritance(
            this Type type,
            bool includeInterfaces = false)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            yield return type;

            var underlyingType = type.BaseType;
            while (underlyingType != null)
            {
                yield return underlyingType;
                underlyingType = underlyingType.BaseType;
            }

            if (!includeInterfaces)
                yield break;

            foreach (var interfaceType in type.GetInterfaces())
                yield return interfaceType;
        }

        public static MethodInfo GetFirstDeclaredMethod(
            this Type type,
            string methodName,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullOrWhiteSpaceException(nameof(methodName));

            foreach (var t in type.EnumerateTypeInheritance(true))
            {
                var typeInfo = t.GetTypeInfo();
                var declaredMethod = typeInfo
                    .DeclaredMethods
                    .FirstOrDefault(o => o.Name.Equals(methodName, stringComparison));

                if (declaredMethod != null)
                    return declaredMethod;
            }

            return null;
        }

        public static bool IsGenericType(this Type type, Type genericType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }
    }
}
