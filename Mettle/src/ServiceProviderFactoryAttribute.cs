using System;

namespace Mettle
{
    [System.AttributeUsage(System.AttributeTargets.Assembly |
        System.AttributeTargets.Class |
        System.AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class ServiceProviderFactoryAttribute : System.Attribute
    {
        public ServiceProviderFactoryAttribute(Type type)
        {
            this.FactoryType = type;
        }

        public Type FactoryType { get; set; }
    }
}