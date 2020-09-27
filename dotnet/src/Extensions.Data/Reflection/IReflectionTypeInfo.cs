using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionTypeInfo : IReflectionMember
    {
        bool IsGenericType { get; }

        IReadOnlyList<Type> Interfaces { get; }

        IReadOnlyList<IReflectionField> Fields { get; }

        IReadOnlyList<IReflectionProperty> Properties { get; }

        IReadOnlyList<IReflectionMethod> Methods { get; }

        IReadOnlyList<IReflectionConstructor> Constructors { get; }

        IReflectionTypeInfo LoadInterfaces();

        IReflectionTypeInfo LoadFields(
            bool includeStatic = false,
            bool includeInherit = false);

        IReflectionTypeInfo LoadFields(BindingFlags flags);

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