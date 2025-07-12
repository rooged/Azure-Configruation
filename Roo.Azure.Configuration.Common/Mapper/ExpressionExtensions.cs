using System.Collections;
using System.Linq.Expressions;

namespace Roo.Azure.Configuration.Common.Mapper
{
    internal static class ExpressionExtensions
    {
        internal static Expression ForEach(this Expression collection, ParameterExpression loopVariable, Expression loopContent)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(loopVariable.Type);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(loopVariable.Type);
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator")!);
            var enumeratorVariable = Expression.Variable(enumeratorType, "enumerator");
            var moveNextCall = Expression.Call(enumeratorVariable, typeof(IEnumerator).GetMethod("MoveNext")!);
            var breakLabel = Expression.Label("LoopBreak");
            return Expression.Block(new[] { enumeratorVariable }, Expression.Assign(enumeratorVariable, getEnumeratorCall), Expression.Loop(Expression.IfThenElse(Expression.IsFalse(moveNextCall),
                Expression.Break(breakLabel), Expression.Block(new[] { loopVariable }, Expression.Assign(loopVariable, Expression.Property(enumeratorVariable, "Current")), loopContent)), breakLabel));
        }
    }
}
