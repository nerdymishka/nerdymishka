using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NerdyMishka.Linq.Expressions
{
    public static class PropertyExpressions
    {

        public static LambdaExpression CreateGetter(Type instanceType, Type propertyType, string propertyName)
        {
            if (propertyType == null)
                throw new ArgumentNullException(nameof(propertyType));

            if (instanceType == null)
            {
                var invokeGet1 = Expression.Property(null, propertyType, propertyName);
                return Expression
                    .Lambda(Expression.Block(invokeGet1));
            }

            var oVariable = Expression.Parameter(instanceType, "o");
            var invokeGet = Expression.Property(oVariable, propertyType, propertyName);
            var b = Expression.Block(invokeGet);
            return Expression
                .Lambda(b, oVariable);
        }

        public static LambdaExpression CreateGetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            if (!propertyInfo.CanRead || propertyInfo.GetMethod is null)
                throw new ArgumentException(
                    $"propertyInfo for {propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name} does not have a get method");

            var isStatic = propertyInfo.GetMethod.IsStatic;
            if (isStatic)
            {
                var invokeGet1 = Expression.Property(null, propertyInfo);
                return Expression
                    .Lambda(Expression.Block(invokeGet1));
            }

            if (propertyInfo.DeclaringType is null)
                throw new ArgumentException($"propertyInfo has no value for DeclaringType");

            var oVariable = Expression.Parameter(propertyInfo.DeclaringType, "o");
            var invokeGet = Expression.Property(oVariable, propertyInfo);
            var b = Expression.Block(invokeGet);
            return Expression
                .Lambda(b, oVariable);
        }

        public static LambdaExpression CreateSetter(Type instanceType, Type propertyType, string propertyName)
        {
            if (propertyType is null)
                throw new ArgumentNullException(nameof(propertyType));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullOrWhiteSpaceException(nameof(propertyName));

            if (instanceType is null)
            {
                var invokeSet = Expression.Property(null, propertyType, propertyName);
                var valueVariable = Expression.Variable(propertyType, "value");
                var b = Expression.Block(Expression.Assign(invokeSet, valueVariable));
                return Expression
                    .Lambda(b, valueVariable);
            }
            else
            {
                var oVariable = Expression.Parameter(instanceType, "o");
                var invokeSet = Expression.Property(oVariable, propertyType, propertyName);
                var valueVariable = Expression.Variable(propertyType, "value");
                var b = Expression.Block(
                    Expression.Assign(invokeSet, valueVariable));

                return Expression
                    .Lambda(b, oVariable, valueVariable);
            }
        }

        public static LambdaExpression CreateSetter(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            if (!propertyInfo.CanWrite || propertyInfo.SetMethod is null)
                throw new ArgumentException(
                    $"propertyInfo for {propertyInfo.DeclaringType?.FullName}.{propertyInfo.Name} does not have a set method");

            if (propertyInfo.SetMethod.IsStatic)
            {
                var invokeSet = Expression.Property(null, propertyInfo);
                var valueVariable = Expression.Variable(propertyInfo.PropertyType, "value");
                var b = Expression.Block(Expression.Assign(invokeSet, valueVariable));
                return Expression
                    .Lambda(b, valueVariable);
            }
            else
            {
                if (propertyInfo.DeclaringType is null)
                    throw new ArgumentException($"propertyInfo has no value for DeclaringType");

                var oVariable = Expression.Parameter(propertyInfo.DeclaringType, "o");
                var invokeSet = Expression.Property(oVariable, propertyInfo);
                var valueVariable = Expression.Variable(propertyInfo.PropertyType, "value");
                var b = Expression.Block(
                    Expression.Assign(invokeSet, valueVariable));

                return Expression
                    .Lambda(b, oVariable, valueVariable);
            }
        }
    }
}
