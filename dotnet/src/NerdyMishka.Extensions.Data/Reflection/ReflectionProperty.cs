using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionProperty : ReflectionMember,
        IReflectionProperty
    {
        private Delegate getter;
        private Delegate setter;
        private bool isStatic = false;

        public ReflectionProperty(PropertyInfo info, IReflectionTypeInfo delcaringType = null)
        {
            this.PropertyInfo = info ?? throw new ArgumentNullException(nameof(info));
            this.DeclaringType = delcaringType;
            this.isStatic = this.PropertyInfo.GetMethod.IsStatic;
        }

        public override string Name => this.PropertyInfo.Name;

        public override Type ClrType => this.PropertyInfo.PropertyType;

        public bool CanRead => this.PropertyInfo.CanRead;

        public bool CanWrite => this.PropertyInfo.CanWrite;

        public PropertyInfo PropertyInfo { get; protected set; }

        public IReflectionTypeInfo DeclaringType { get; protected set; }

        public static Delegate CreateGetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var isStatic = propertyInfo.GetMethod.IsStatic;
            if (isStatic)
            {
                var invokeGet1 = Expression.Property(null, propertyInfo);
                return Expression
                    .Lambda(Expression.Block(invokeGet1))
                    .Compile();
            }

            var oVariable = Expression.Parameter(propertyInfo.DeclaringType, "o");
            var invokeGet = Expression.Property(oVariable, propertyInfo);
            var b = Expression.Block(invokeGet);
            return Expression
                .Lambda(b, oVariable)
                .Compile();
        }

        public static Delegate CreateSetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                throw new ArgumentNullException(nameof(propertyInfo));

            if (propertyInfo.SetMethod is null)
                throw new ArgumentException(
                    $"propertyInfo for {propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name} does not have a set method");

            if (propertyInfo.SetMethod.IsStatic)
            {
                var invokeSet = Expression.Property(null, propertyInfo);
                var valueVariable = Expression.Variable(propertyInfo.PropertyType, "value");
                var b = Expression.Block(Expression.Assign(invokeSet, valueVariable));
                return Expression
                    .Lambda(b, valueVariable)
                    .Compile();
            }
            else
            {
                var oVariable = Expression.Parameter(propertyInfo.DeclaringType, "o");
                var invokeSet = Expression.Property(oVariable, propertyInfo);
                var valueVariable = Expression.Variable(propertyInfo.PropertyType, "value");
                var b = Expression.Block(
                    Expression.Assign(invokeSet, valueVariable));

                return Expression
                    .Lambda(b, oVariable, valueVariable)
                    .Compile();
            }
        }

        public virtual object GetValue(object instance)
        {
            if (!this.CanRead)
                throw new InvalidOperationException($"Property {this.Name} prohibits reading the value.");

            if (this.getter == null)
            {
                this.getter = CreateGetter(this.PropertyInfo);
            }

            if (this.isStatic)
                return this.getter.DynamicInvoke();

            return this.getter.DynamicInvoke(instance);
        }

        public virtual void SetValue(object instance, object value)
        {
            if (!this.CanWrite)
                throw new InvalidOperationException($"Property {this.Name} prohibits writing the value.");

            if (this.setter == null)
            {
                this.setter = CreateSetter(this.PropertyInfo);
            }

            if (this.isStatic)
            {
                this.setter.DynamicInvoke(value);
                return;
            }

            this.setter.DynamicInvoke(instance, value);
        }

        public override void LoadAttributes(bool inherit = true)
        {
            this.SetAttributes(
                CustomAttributeExtensions.GetCustomAttributes(this.PropertyInfo, inherit));
        }
    }
}