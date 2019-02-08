using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MyPerfectOnboarding.Contracts.Selector
{
    internal static class ExpressionExtensions
    {
        public static PropertyInfo ToPropertyInfo<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertySelector)
        {
            var member = (MemberExpression)propertySelector.Body;
            var propertyInfo = (PropertyInfo)(member).Member;

            return propertyInfo;
        }
    }
}
