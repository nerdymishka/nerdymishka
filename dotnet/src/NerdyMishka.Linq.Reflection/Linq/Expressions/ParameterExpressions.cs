using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Linq.Expressions
{
    public static class ParameterExpressions
    {
        public static ValueTuple<ParameterExpression, IReadOnlyCollection<Expression>>
            CreateArgumentList(ParameterInfo[] parameters)
        {
            var parameter = Expression.Parameter(typeof(object[]), "parameters");
            var list = new List<Expression>();
            if (parameters is null || parameters.Length == 0)
                return ValueTuple.Create(parameter, list);

            foreach (var p in parameters)
            {
                list.Add(
                    Expression.Convert(
                        Expression.ArrayIndex(parameter,
                            Expression.Constant(p.Position)),
                        p.ParameterType));
            }

            return ValueTuple.Create(parameter, list);
        }
    }
}
