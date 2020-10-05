using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionConstructor : IReflectionMethodBaseShared,
        IReflectionMember
    {
        ConstructorInfo ConstructorInfo { get; }

        object Invoke(params object[] parameters);
    }
}