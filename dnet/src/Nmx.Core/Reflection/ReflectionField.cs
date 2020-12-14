using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionField : ReflectionMember, IReflectionField
    {
        private Delegate getter;

        private Delegate setter;

        public ReflectionField(FieldInfo info, IReflectionTypeInfo delcaringType = null)
        {
            this.FieldInfo = info ?? throw new ArgumentNullException(nameof(info));
            this.DeclaringType = delcaringType;
        }

        public IReflectionTypeInfo DeclaringType { get; protected set; }

        public FieldInfo FieldInfo { get; protected set; }

        public override string Name => this.FieldInfo.Name;

        public override Type ClrType => this.FieldInfo.FieldType;

        public bool CanRead => true;

        public bool CanWrite => !this.FieldInfo.IsInitOnly;

        public static Delegate CreateGetter(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            MemberExpression invokeGet;
            if (fieldInfo.IsStatic)
            {
                invokeGet = Expression.Field(null, fieldInfo);
                return Expression
                    .Lambda(Expression.Block(invokeGet))
                    .Compile();
            }
            else
            {
                var oVariable = Expression.Parameter(fieldInfo.DeclaringType, "o");
                invokeGet = Expression.Field(oVariable, fieldInfo);
                return Expression
                    .Lambda(Expression.Block(invokeGet), oVariable)
                    .Compile();
            }
        }

        public static Delegate CreateSetter(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                throw new ArgumentNullException(nameof(fieldInfo));

            if (fieldInfo.IsStatic)
            {
                var invokeSet = Expression.Field(null, fieldInfo);
                var valueVariable = Expression.Variable(fieldInfo.FieldType, "value");
                var b = Expression.Block(
                    Expression.Assign(invokeSet, valueVariable));

                return Expression
                    .Lambda(b, valueVariable)
                    .Compile();
            }
            else
            {
                var oVariable = Expression.Parameter(fieldInfo.DeclaringType, "o");
                var invokeSet = Expression.Field(oVariable, fieldInfo);
                var valueVariable = Expression.Variable(fieldInfo.FieldType, "value");
                var b = Expression.Block(
                    Expression.Assign(invokeSet, valueVariable));

                return Expression
                    .Lambda(b, oVariable, valueVariable)
                    .Compile();
            }
        }

        public virtual object GetValue(object instance)
        {
            if (this.getter == null)
                this.getter = CreateGetter(this.FieldInfo);

            if (!this.FieldInfo.IsStatic)
                return this.getter.DynamicInvoke(instance);

            return this.getter.DynamicInvoke();
        }

        public virtual void SetValue(object instance, object value)
        {
            if (!this.CanWrite)
                throw new InvalidOperationException($"Property {this.Name} prohibits reading the value.");

            if (this.setter == null)
                this.setter = CreateSetter(this.FieldInfo);

            if (!this.FieldInfo.IsStatic)
                this.setter.DynamicInvoke(instance, value);
            else
                this.setter.DynamicInvoke(value);
        }

        public override void LoadAttributes(bool inherit = true)
        {
            this.SetAttributes(
                CustomAttributeExtensions.GetCustomAttributes(this.FieldInfo, inherit));
        }
    }
}