using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionMethod : IReflectionMethodBaseShared,
        IReflectionMember
    {
        MethodInfo MethodInfo { get; }

        IReflectionParameter ReturnParameter { get; }

        object Invoke(object instance, params object[] parameters);
    }
}