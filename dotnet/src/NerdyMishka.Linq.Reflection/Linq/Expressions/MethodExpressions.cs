using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Linq.Expressions
{
    public static class MethodExpressions
    {
        public static Expression<Action<object, object[]>> CreateVoidMethodExpressionWithArgs(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (method.IsStatic || method.ReflectedType == null)
                throw new ArgumentException($"methodInfo for {method.Name} is static or missing ReflectedType");

            if (method.ReturnType != typeof(void))
                throw new ArgumentException($"method {method.Name} return type is not void");

            var instanceExpression = Expression.Parameter(typeof(object), "obj");
            var (parameter, parameters) = ParameterExpressions
                .CreateArgumentList(method.GetParameters());

            var unary = Expression.Convert(instanceExpression, method.ReflectedType);

            var callExpression = Expression.Call(
                unary,
                method,
                parameters);

            return Expression.Lambda<Action<object, object[]>>(
                Expression.Convert(
                    callExpression,
                    typeof(object)),
                instanceExpression,
                parameter);
        }

        public static Expression<Func<object, object>> CreateMethodExpression(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (method.IsStatic || method.ReflectedType == null)
                throw new ArgumentException($"methodInfo for {method.Name} is static or missing ReflectedType");

            var instanceExpression = Expression.Parameter(typeof(object), "obj");
            var unary = Expression.Convert(instanceExpression, method.ReflectedType);

            var callExpression = Expression.Call(
                unary,
                method);

            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    callExpression,
                    typeof(object)),
                instanceExpression);
        }

        public static Expression<Func<object, TReturn>> CreateMethodExpression<TReturn>(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (method.IsStatic || method.ReflectedType == null)
                throw new ArgumentException($"methodInfo for {method.Name} is static or missing ReflectedType");

            var instanceExpression = Expression.Parameter(typeof(object), "obj");
            var unary = Expression.Convert(instanceExpression, method.ReflectedType);

            var callExpression = Expression.Call(
                unary,
                method);

            return Expression.Lambda<Func<object, TReturn>>(
                Expression.Convert(
                    callExpression,
                    typeof(TReturn)),
                instanceExpression);
        }

        public static Expression<Func<object, object[], TReturn>> CreateMethodExpressionWithArgs<TReturn>(MethodInfo method)
        {
            // based upon code from stack overflow.
            // https://stackoverflow.com/questions/10313979/methodinfo-invoke-performance-issue
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (method.IsStatic || method.ReflectedType == null)
                throw new ArgumentException($"methodInfo for {method.Name} is static or missing ReflectedType");

            var instanceExpression = Expression.Parameter(typeof(object), "obj");
            var (parameter, parameters) = ParameterExpressions
                .CreateArgumentList(method.GetParameters());

            var unary = Expression.Convert(instanceExpression, method.ReflectedType);

            var callExpression = Expression.Call(
                unary,
                method,
                parameters);

            return Expression.Lambda<Func<object, object[], TReturn>>(
                Expression.Convert(
                    callExpression,
                    typeof(TReturn)),
                instanceExpression,
                parameter);
        }

        public static Expression<Func<object, object[], object>> CreateMethodExpressionWithArgs(MethodInfo method)
        {
            // based upon code from stack overflow.
            // https://stackoverflow.com/questions/10313979/methodinfo-invoke-performance-issue
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (method.IsStatic || method.ReflectedType == null)
                throw new ArgumentException($"methodInfo for {method.Name} is static or missing ReflectedType");

            var instanceExpression = Expression.Parameter(typeof(object), "obj");
            var (parameter, parameters) = ParameterExpressions
                .CreateArgumentList(method.GetParameters());

            var unary = Expression.Convert(instanceExpression, method.ReflectedType);

            var callExpression = Expression.Call(
                unary,
                method,
                parameters);

            return Expression.Lambda<Func<object, object[], object>>(
                Expression.Convert(
                    callExpression,
                    typeof(object)),
                instanceExpression,
                parameter);
        }

        public static Func<object, object[], object> CreateMethodCallerWithArgs(MethodInfo method)
        {
            // based upon code from stack overflow.
            // https://stackoverflow.com/questions/10313979/methodinfo-invoke-performance-issue
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (method.IsStatic || method.ReflectedType == null)
                throw new ArgumentException($"methodInfo for {method.Name} is static or missing ReflectedType");

            var instanceExpression = Expression.Parameter(typeof(object), "obj");
            var (parameter, parameters) = ParameterExpressions
                .CreateArgumentList(method.GetParameters());

            var unary = Expression.Convert(instanceExpression, method.ReflectedType);

            var callExpression = Expression.Call(
                unary,
                method,
                parameters);

            if (callExpression.Type != typeof(void))
                return Expression.Lambda<Func<object, object[], object>>(
                        Expression.Convert(
                            callExpression,
                            typeof(object)),
                        instanceExpression,
                        parameter)
                    .Compile();

            var voidDelegate = Expression.Lambda<Action<object, object[]>>(
                    callExpression,
                    instanceExpression,
                    parameter)
                .Compile();

            object Action(object obj, object[] arguments)
            {
                voidDelegate(obj, arguments);
                return null;
            }

            return Action;
        }

        public static Delegate CreateMethodDelegate(MethodInfo method)
            => CreateMethodCallerWithArgs(method);
    }
}
