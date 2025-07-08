using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Roo.Azure.Configuration.Common.Mapper
{
    /// <summary>
    /// Builds precompiled mapping expressions for converting between source and destination types.
    /// </summary>
    public static class MappingExpressionBuilder
    {
        /// <summary>
        /// Builds a mapping function that converts from source to destination using the provided <see cref="RooMapper"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="customMaps"></param>
        /// <param name="ignoredProperties"></param>
        /// <param name="compiledMapCache"></param>
        /// <returns></returns>
        public static Func<TSource, TDestination> BuildMapFunction<TSource, TDestination>(RooMapper mapper, Dictionary<string, LambdaExpression>? customMaps = null,
            HashSet<string>? ignoredProperties = null, Dictionary<(Type, Type, string, string), Delegate>? compiledMapCache = null) where TDestination : new()
        {
            compiledMapCache ??= new();
            var key = (typeof(TSource), typeof(TDestination), customMaps != null ? string.Join(",", customMaps.Keys.OrderBy(x => x)) : "", ignoredProperties != null ? string.Join(",", ignoredProperties.OrderBy(x => x)) : "");
            if (compiledMapCache.TryGetValue(key, out var existingMap))
            {
                return (Func<TSource, TDestination>)existingMap;
            }

            var sourceParameter = Expression.Parameter(typeof(TSource), "src");
            var destinationVariable = Expression.Parameter(typeof(TDestination), "dest");
            var assignExpression = new List<Expression> { Expression.Assign(destinationVariable, Expression.New(typeof(TDestination))) };

            foreach (var destinationProperty in typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite))
            {
                var destinationPropertyName = destinationProperty.Name;

                //if in ignored list, skip
                if (ignoredProperties != null && ignoredProperties.Contains(destinationPropertyName))
                {
                    continue;
                }

                //If custom map exists, add expression and skip
                if (customMaps != null && customMaps.TryGetValue(destinationPropertyName, out var customExpression))
                {
                    var replaced = new ParameterReplaceVisitor(customExpression.Parameters[0], sourceParameter).Visit(customExpression.Body);
                    assignExpression.Add(Expression.Assign(Expression.Property(destinationVariable, destinationProperty), Expression.Convert(replaced!, destinationProperty.PropertyType)));
                    continue;
                }

                //If source property is null or can't be read, skip
                var sourceProperty = typeof(TSource).GetProperty(destinationPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (sourceProperty == null || !sourceProperty.CanRead)
                {
                    continue;
                }
                var sourcePropertyType = sourceProperty.PropertyType;
                var destinationPropertyType = destinationProperty.PropertyType;

                //Build collection type maps
                if (destinationPropertyType.IsCollectionType(out var destinationElementType) && destinationPropertyType != typeof(string) && sourcePropertyType.IsCollectionType(out var sourceElementType) && sourcePropertyType != typeof(string))
                {
                    var elementMapDelegate = mapper.GetMappingDelegate(sourceElementType, destinationElementType);
                    var elementMapFunction = new Func<object, object>(src => elementMapDelegate(src));
                    var sourcePropertyExpression = Expression.Property(sourceParameter, sourceProperty);
                    var mapCollectionExpression = BuildMapCollectionExpression(sourcePropertyExpression, destinationPropertyType, sourceElementType, destinationElementType, elementMapDelegate);
                    assignExpression.Add(Expression.Assign(Expression.Property(destinationVariable, destinationProperty), Expression.Convert(mapCollectionExpression, destinationPropertyType)));
                    continue;
                }
                //Build complex type maps
                if (destinationPropertyType.IsComplexType() && sourcePropertyType.IsComplexType())
                {
                    //Filter to current level nested maps
                    Dictionary<string, LambdaExpression>? nestedCustomMaps = null;
                    if (customMaps != null)
                    {
                        var prefix = destinationPropertyName + ".";
                        nestedCustomMaps = customMaps.Where(x => x.Key.StartsWith(prefix)).ToDictionary(x => x.Key.Substring(prefix.Length), x => x.Value);
                    }
                    HashSet<string>? nestedIgnored = null;
                    if (ignoredProperties != null)
                    {
                        var prefix = destinationPropertyName + ".";
                        nestedIgnored = ignoredProperties.Where(x => x.StartsWith(prefix)).Select(x => x.Substring(prefix.Length)).ToHashSet();
                    }
                    var nestedMapFunction = GetOrBuildMapFunction(sourcePropertyType, destinationPropertyType, mapper, nestedCustomMaps, nestedIgnored, compiledMapCache);
                    var sourcePropertyExpression = Expression.Property(sourceParameter, sourceProperty);
                    var assignNested = Expression.Assign(Expression.Property(destinationVariable, destinationProperty), Expression.Condition(Expression.Equal(sourcePropertyExpression, Expression.Constant(null, sourcePropertyType)),
                        Expression.Constant(null, destinationPropertyType), Expression.Convert(Expression.Invoke(Expression.Constant(nestedMapFunction), sourcePropertyExpression), destinationPropertyType)));
                    assignExpression.Add(assignNested);
                    continue;
                }
                //Direct assignment
                if (destinationPropertyType.IsAssignableFrom(sourcePropertyType))
                {
                    var sourceExpression = Expression.Property(sourceParameter, sourceProperty);
                    Expression assignValue;
                    var underlyingType = Nullable.GetUnderlyingType(destinationPropertyType);
                    if (underlyingType != null && underlyingType == sourcePropertyType)
                    {
                        assignValue = Expression.Convert(sourceExpression, destinationPropertyType);
                    }
                    else
                    {
                        assignValue = sourceExpression;
                    }
                    assignExpression.Add(Expression.Assign(Expression.Property(destinationVariable, destinationProperty), assignValue));
                }
            }
            assignExpression.Add(destinationVariable);
            var body = Expression.Block(new[] { destinationVariable }, assignExpression);
            var lambda = Expression.Lambda<Func<TSource, TDestination>>(body, sourceParameter);
            var compiled = lambda.Compile();
            compiledMapCache[key] = compiled;
            return compiled;
        }

        private static Delegate GetOrBuildMapFunction(Type sourceType, Type destinationType, RooMapper mapper, Dictionary<string, LambdaExpression>? customMaps, HashSet<string>? ignoredProperties, Dictionary<(Type, Type, string, string), Delegate> cache)
        {
            var key = (sourceType, destinationType, customMaps != null ? string.Join(",", customMaps.Keys.OrderBy(x => x)) : "", ignoredProperties != null ? string.Join(",", ignoredProperties.OrderBy(x => x)) : "");
            if (cache.TryGetValue(key, out var del))
            {
                return del;
            }
            var method = typeof(MappingExpressionBuilder).GetMethod(nameof(BuildMapFunction), BindingFlags.Static | BindingFlags.Public)!.MakeGenericMethod(sourceType, destinationType);
            var function = (Delegate)method.Invoke(null, new object[] { mapper, customMaps, ignoredProperties, cache })!;
            cache[key] = function;
            return function;
        }

        private static Expression BuildMapCollectionExpression(Expression sourcePropertyExpression, Type destinationCollectionType, Type sourceElementType, Type destinationElementType, object elementMapFunction)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(sourceElementType);
            var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!.MakeGenericMethod(sourceElementType);

            var sourceListVariable = Expression.Variable(typeof(List<>).MakeGenericType(sourceElementType), "srcList");
            var sourceListVariableType = sourceListVariable.Type;
            var assignSourceList = Expression.Assign(sourceListVariable, Expression.Condition(Expression.Equal(sourcePropertyExpression, Expression.Constant(null, sourcePropertyExpression.Type)),
                Expression.Constant(null, sourceListVariableType), Expression.Call(toListMethod, sourcePropertyExpression)));

            var destinationListVariable = Expression.Variable(typeof(List<>).MakeGenericType(destinationElementType), "destList");
            var assignDestinationList = Expression.Assign(destinationListVariable, Expression.New(destinationListVariable.Type));

            var itemVariable = Expression.Variable(sourceElementType, "item");
            Expression binaryExpression;
            if (!sourceElementType.IsValueType || Nullable.GetUnderlyingType(sourceElementType) != null)
            {
                binaryExpression = Expression.Equal(itemVariable, Expression.Constant(null, sourceElementType));
            }
            else
            {
                binaryExpression = Expression.Constant(false, typeof(bool));
            }
            Expression expressionCallIfTrue;
            if (!destinationElementType.IsValueType || Nullable.GetUnderlyingType(destinationElementType) != null)
            {
                expressionCallIfTrue = Expression.Call(destinationListVariable, destinationListVariable.Type.GetMethod("Add")!, Expression.Constant(null, destinationElementType));
            }
            else
            {
                expressionCallIfTrue = Expression.Empty();
            }
            var expressionCallIfFalse = Expression.Call(destinationListVariable, destinationListVariable.Type.GetMethod("Add")!, Expression.Convert(Expression.Invoke(Expression.Constant(elementMapFunction), Expression.Convert(itemVariable, typeof(object))), destinationElementType));
            var loopBody = Expression.IfThenElse(binaryExpression, expressionCallIfTrue, expressionCallIfFalse);
            var loop = ForEach(sourceListVariable, itemVariable, loopBody);

            Expression result;
            if (destinationCollectionType.IsArray)
            {
                var toArrayMethod = destinationListVariable.Type.GetMethod("ToArray")!;
                result = Expression.Condition(Expression.Equal(sourceListVariable, Expression.Constant(null, sourceListVariableType)), Expression.Constant(null, destinationCollectionType), Expression.Call(destinationListVariable, toArrayMethod));
            }
            else if (!destinationCollectionType.IsAssignableFrom(destinationListVariable.Type))
            {
                var constrcutor = destinationCollectionType.GetConstructor(new[] { destinationListVariable.Type });
                if (constrcutor != null)
                {
                    result = Expression.Condition(Expression.Equal(sourceListVariable, Expression.Constant(null, sourceListVariableType)), Expression.Constant(null, destinationCollectionType), Expression.New(constrcutor, destinationListVariable));
                }
                else
                {
                    result = Expression.Condition(Expression.Equal(sourceListVariable, Expression.Constant(null, sourceListVariableType)), Expression.Constant(null, destinationCollectionType), destinationListVariable);
                }
            }
            else
            {
                result = Expression.Condition(Expression.Equal(sourceListVariable, Expression.Constant(null, sourceListVariableType)), Expression.Constant(null, destinationCollectionType), destinationListVariable);
            }
            return Expression.Block(new[] { sourceListVariable, destinationListVariable }, assignSourceList, assignDestinationList, Expression.IfThen(Expression.NotEqual(sourceListVariable, Expression.Constant(null, sourceListVariableType)), loop), result);
        }

        private static Expression ForEach(ParameterExpression collection, ParameterExpression loopVariable, Expression body)
        {
            var getEnumerator = typeof(IEnumerable<>).MakeGenericType(loopVariable.Type).GetMethod("GetEnumerator")!;
            var enumeratorVariable = Expression.Variable(getEnumerator.ReturnType, "enumerator");
            var moveNext = typeof(IEnumerator).GetMethod("MoveNext")!;
            var current = getEnumerator.ReturnType.GetProperty("Current")!;
            var breakLabel = Expression.Label("break");
            return Expression.Block(new[] { enumeratorVariable }, Expression.Assign(enumeratorVariable, Expression.Call(collection, getEnumerator)),
                Expression.Loop(Expression.IfThenElse(Expression.Call(enumeratorVariable, moveNext),
                Expression.Block(new[] { loopVariable }, Expression.Assign(loopVariable, Expression.Property(enumeratorVariable, current)), body),
                Expression.Break(breakLabel)), breakLabel));
        }
    }

    internal class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly Expression _to;

        public ParameterReplaceVisitor(ParameterExpression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) => node == _from ? _to : base.VisitParameter(node);
    }
}
