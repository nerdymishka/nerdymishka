using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionConstructor : ReflectionMethodBase,
        IReflectionConstructor
    {
        private Delegate ctor;

        public ReflectionConstructor(
            ConstructorInfo constructorInfo,
            IReflectionFactory factory,
            ParameterInfo[] parameters = null,
            IReflectionTypeInfo declaringType = null)
            : base(constructorInfo, factory, parameters, declaringType)
        {
            this.ConstructorInfo = constructorInfo;
        }

        public ConstructorInfo ConstructorInfo { get; private set; }

        public static Delegate CreateDelegate(ConstructorInfo ctor)
        {
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));

            var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
            var argumentExpressions = new List<Expression>();

            foreach (var parameter in ctor.GetParameters())
            {
                argumentExpressions.Add(
                    Expression.Convert(
                        Expression.ArrayIndex(argumentsExpression,
                        Expression.Constant(parameter.Position)),
                        parameter.ParameterType));
            }

            NewExpression newExpression;
            if (argumentExpressions.Count == 0)
                newExpression = Expression.New(ctor);
            else
                newExpression =
                    Expression.New(ctor, argumentExpressions.ToArray());

            return Expression.Lambda<Func<object[], object>>(
                Expression.Convert(newExpression, typeof(object)),
                argumentsExpression).Compile();
        }

        public object Invoke(params object[] parameters)
        {
            if (this.ctor != null)
                return this.ctor.DynamicInvoke(parameters);

            this.ctor = CreateDelegate(this.ConstructorInfo);
            return this.ctor.DynamicInvoke(parameters);
        }
    }
}