using System;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    /// <summary>
    /// The core contract for all reflection metadata.
    /// </summary>
    /// <typeparam name="T">The type that inherits <see cref="IReflectionMember{T}"/>.</typeparam>
    public interface IReflectionMember
    {
        IReadOnlyList<Attribute> Attributes { get; }

        string Name { get; }

        Type ClrType { get; }

        TValue GetAnnotation<TValue>(string name);

        void SetAnnotation<TValue>(string name, TValue value);

        void LoadAttributes(bool inherit = true);

        TAttribute FindAttribute<TAttribute>()
            where TAttribute : Attribute;

        IReadOnlyList<TAttribute> FindAttributes<TAttribute>()
            where TAttribute : Attribute;
    }
}