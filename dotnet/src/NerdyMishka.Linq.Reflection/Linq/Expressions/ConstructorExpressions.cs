using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Linq.Expressions
{
    public static class ConstructorExpressions
    {
        public static Func<object> CreateActivator(ConstructorInfo ctor)
        {
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));

            return Expression.Lambda<Func<object>>(
                    Expression.Convert(Expression.New(ctor), typeof(object)))
                .Compile();
        }

        public static Func<T> CreateActivator<T>(ConstructorInfo ctor)
        {
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));

            return Expression.Lambda<Func<T>>(
                    Expression.Convert(Expression.New(ctor), typeof(T)))
                .Compile();
        }

        public static Func<object[], object> CreateActivatorWithArgs(ConstructorInfo ctor)
        {
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));

            var (parameter, arguments) = ParameterExpressions.CreateArgumentList(ctor.GetParameters());

            var newExpression = arguments.Count == 0
                ? Expression.New(ctor)
                : Expression.New(ctor, arguments.ToArray());

            return Expression.Lambda<Func<object[], object>>(
                    Expression.Convert(newExpression, typeof(object)), parameter)
                .Compile();
        }

        public static Func<object[], T> CreateActivatorWithArgs<T>(ConstructorInfo ctor)
        {
            if (ctor is null)
                throw new ArgumentNullException(nameof(ctor));
            var (parameter, arguments) = ParameterExpressions.CreateArgumentList(ctor.GetParameters());

            var newExpression = arguments.Count == 0
                ? Expression.New(ctor)
                : Expression.New(ctor, arguments.ToArray());

            return Expression.Lambda<Func<object[], T>>(
                    Expression.Convert(newExpression, typeof(T)), parameter)
                .Compile();
        }
    }
}
