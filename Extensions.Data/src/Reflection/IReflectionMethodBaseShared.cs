using System;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    public interface IReflectionMethodBaseShared
    {
        IReadOnlyList<IReflectionParameter> Parameters { get; }

        IReadOnlyList<Type> GenericArguments { get; }

        IReadOnlyList<Type> ParameterTypes { get; }

        bool IsGenericMethodDefinition { get; }
    }
}