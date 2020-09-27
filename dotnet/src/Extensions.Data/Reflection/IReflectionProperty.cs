using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionProperty :
        IReflectionMember,
        IReflectionValueAccessor
    {
        PropertyInfo PropertyInfo { get; }

        IReflectionTypeInfo DeclaringType { get; }

        bool CanRead { get; }

        bool CanWrite { get; }
    }
}