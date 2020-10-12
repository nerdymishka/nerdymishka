using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Linq.Expressions
{
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression oldExpression;
        private readonly Expression newExpression;

        public ReplaceExpressionVisitor(Expression oldExpression, Expression newExpression)
        {
            this.oldExpression = oldExpression;
            this.newExpression = newExpression;
        }

        public override Expression Visit(Expression node)
        {
            if (this.oldExpression == node)
                node = this.newExpression;

            return base.Visit(node);
        }
    }
}
