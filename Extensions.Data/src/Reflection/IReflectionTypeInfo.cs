using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionTypeInfo : IReflectionMember
    {
        IReadOnlyList<Type> Interfaces { get; }

        IReadOnlyList<IReflectionProperty> Properties { get; }

        IReadOnlyList<IReflectionMethod> Methods { get; }

        IReadOnlyList<IReflectionConstructor> Constructors { get; }

        IReflectionTypeInfo LoadInterfaces();

        IReflectionTypeInfo LoadProperties(
            bool includeStatic = false,
            bool includeInherit = false);

        IReflectionTypeInfo LoadProperties(BindingFlags flags);

        IReflectionTypeInfo LoadMethods(
            bool includeStatic = false,
            bool includeInherit = false);

        IReflectionTypeInfo LoadMethods(BindingFlags flags);
    }
}