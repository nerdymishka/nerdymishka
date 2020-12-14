using System;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionParameter : ReflectionMember, IReflectionParameter
    {
        public ReflectionParameter(ParameterInfo info)
        {
            this.ParameterInfo = info ?? throw new ArgumentNullException(nameof(info));
        }

        public IReflectionTypeInfo DeclaringType { get; }

        public override string Name => this.ParameterInfo.Name;

        public override Type ClrType => this.ParameterInfo.ParameterType;

        public object DefaultValue => this.ParameterInfo.DefaultValue;

        public int Position => this.ParameterInfo.Position;

        public bool IsOut => this.ParameterInfo.IsOut;

        public bool IsOptional => this.ParameterInfo.IsOptional;

        public ParameterInfo ParameterInfo { get; protected set; }

        public override void LoadAttributes(bool inherit = true)
        {
            this.SetAttributes(
                CustomAttributeExtensions
                    .GetCustomAttributes(this.ParameterInfo, inherit));
        }
    }
}