using System;

namespace NerdyMishka.Reflection
{
    public interface IReflectionCache : IDisposable
    {
        IReflectionFactory Factory { get; }

        void Clear();

        bool TryRemove(IReflectionTypeInfo typeInfo);

        IReflectionTypeInfo GetOrAdd(Type type);
    }
}