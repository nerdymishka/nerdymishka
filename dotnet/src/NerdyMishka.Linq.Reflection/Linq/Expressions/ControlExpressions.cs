using System;
using System.Linq.Expressions;
using NerdyMishka.Reflection;

namespace NerdyMishka.Linq.Expressions
{
    public static class ControlExpressions
    {
        public static Expression NullConstant => Expression.Constant(null);

        public static Expression TrueConstant => Expression.Constant(true);

        public static Expression<Action<IDisposable>> DisposeExpression => (disposable) => disposable.Dispose();

        public static Expression CastToObject(Expression expression)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));

            return expression.Type == typeof(object) ? expression : Expression.Convert(expression, typeof(object));
        }

        public static Expression CastToType(Expression expression, Type type)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));

            return expression.Type == type ? expression : Expression.Convert(expression, type);
        }

        public static Expression IfNull(
            this Expression expression,
            Expression thenExpression,
            Expression elseExpression = null)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));

            if (thenExpression is null)
                throw new ArgumentNullException(nameof(thenExpression));

            elseExpression = elseExpression != null ?
                CastToType(elseExpression, thenExpression.Type) :
                Expression.Default(thenExpression.Type);

            if (expression.Type.IsValueType && !expression.Type.IsGenericType(typeof(Nullable<>)))
                return elseExpression;

            return Expression.Condition(
                Expression.Equal(expression,
                    NullConstant),
                thenExpression,
                elseExpression);
        }

        public static Expression ForEach(
            Expression collectionExpression,
            ParameterExpression parameterExpression,
            Expression bodyExpression)
        {
            if (collectionExpression is null)
                throw new ArgumentNullException(nameof(collectionExpression));

            if (parameterExpression is null)
                throw new ArgumentNullException(nameof(parameterExpression));

            if (bodyExpression is null)
                throw new ArgumentNullException(nameof(bodyExpression));

            if (collectionExpression.Type.IsArray)
            {
                return ArrayFor(
                    collectionExpression,
                    arrayItem =>
                        Expression.Block(
                            new[] { parameterExpression },
                            Expression.Assign(parameterExpression, arrayItem),
                            bodyExpression));
            }

            var getEnumeratorMethodInfo = collectionExpression.Type.GetFirstDeclaredMethod("GetEnumerator");
            var getEnumeratorExpression = Expression.Call(
                collectionExpression,
                getEnumeratorMethodInfo);

            var enumeratorVarExpression = Expression.Variable(
                getEnumeratorExpression.Type,
                "iterator");
            var enumeratorAssignExpression = Expression.Assign(
                enumeratorVarExpression,
                getEnumeratorExpression);

            var moveNextMethodInfo = getEnumeratorExpression
                .Type
                .GetFirstDeclaredMethod("MoveNext");
            var moveNextExpression = Expression.Call(
                enumeratorVarExpression,
                moveNextMethodInfo);

            var currentExpression = CastToType(
                Expression.Property(enumeratorVarExpression, "Current"),
                parameterExpression.Type);

            var breakLabel = Expression.Label("BreakForEach");

            var loopExpression = Expression.Block(
                new[] { enumeratorVarExpression },
                enumeratorAssignExpression,
                Using(
                    enumeratorVarExpression,
                    Expression.Loop(
                       Expression.IfThenElse(
                           Expression.Equal(
                               moveNextExpression,
                               TrueConstant),
                           Expression.Block(
                                new[] { parameterExpression },
                                Expression.Assign(
                                    parameterExpression,
                                    currentExpression),
                                bodyExpression),
                           Expression.Break(breakLabel)),
                       breakLabel)));

            return loopExpression;
        }

        public static Expression ArrayFor(Expression array, Func<Expression, Expression> body)
        {
            var length = Expression.Property(array, "Length");
            return For(length,
                index => body(
                    Expression.ArrayAccess(array, index)));
        }

        public static Expression For(
            Expression countExpression,
            Func<Expression, Expression> bodyExpression)
        {
            if (countExpression is null)
                throw new ArgumentNullException(nameof(countExpression));

            if (bodyExpression is null)
                throw new ArgumentNullException(nameof(bodyExpression));

            var breakExpression = Expression.Label("BreakLoop");
            var indexVarExpression = Expression.Variable(countExpression.Type, "listIndex");
            var assignExpression = Expression.Assign(
                indexVarExpression, Expression.Constant(0, countExpression.Type));
            var loopExpression = Expression.Block(
                new[] { indexVarExpression },
                assignExpression,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(indexVarExpression, countExpression),
                        Expression.Block(
                            bodyExpression(indexVarExpression),
                            Expression.PostIncrementAssign(indexVarExpression)),
                        Expression.Break(breakExpression)),
                    breakExpression));

            return loopExpression;
        }

        public static Expression Replace(
            this Expression expression,
            Expression oldExpression,
            Expression newExpression)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));

            return new ReplaceExpressionVisitor(oldExpression, newExpression).Visit(expression);
        }

        public static Expression ReplaceParameters(
            this LambdaExpression lambdaExpression,
            params Expression[] expressions)
        {
            if (lambdaExpression is null)
                throw new ArgumentNullException(nameof(lambdaExpression));

            var bodyExpression = lambdaExpression.Body;
            var l = Math.Min(expressions.Length, lambdaExpression.Parameters.Count);
            for (var i = 0; i < l; i++)
            {
                bodyExpression = Replace(
                    bodyExpression,
                    lambdaExpression.Parameters[i],
                    expressions[i]);
            }

            return bodyExpression;
        }

        public static Expression Using(Expression disposableExpression, Expression bodyExpression)
        {
            if (disposableExpression is null)
                throw new ArgumentNullException(nameof(disposableExpression));

            if (bodyExpression is null)
                throw new ArgumentNullException(nameof(bodyExpression));

            Expression disposeExpression;
            if (disposableExpression.Type.IsValueType)
            {
                return bodyExpression;
            }

            if (typeof(IDisposable).IsAssignableFrom(disposableExpression.Type))
            {
                disposeExpression = DisposeExpression.ReplaceParameters(disposableExpression);
                return Expression.TryFinally(bodyExpression, disposeExpression);
            }

            var disposableVarExpression =
                Expression.Variable(typeof(IDisposable), "disposable");

            var assignExpression = Expression.Assign(
                disposableVarExpression,
                Expression.TypeAs(disposableExpression, typeof(IDisposable)));

            disposeExpression =
                Expression.Block(
                    new[] { disposableVarExpression },
                    assignExpression,
                    IfNull(
                        disposableVarExpression,
                        Expression.Empty(),
                        DisposeExpression.ReplaceParameters(disposableVarExpression)));

            return Expression.TryFinally(bodyExpression, disposableExpression);
        }
    }
}
