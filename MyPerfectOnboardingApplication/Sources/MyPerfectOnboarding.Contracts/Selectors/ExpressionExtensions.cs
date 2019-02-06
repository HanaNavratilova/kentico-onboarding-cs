using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MyPerfectOnboarding.Contracts.Selectors
{
    internal static class ExpressionExtensions
    {
        public static PropertyInfo ToPropertyInfo<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertySelector)
        {
            if (!(propertySelector.Body is MemberExpression member))
            {
                throw new InvalidOperationException("Property selector body isn't a member expression. It's impossible select a member property.");
            }

            if (!(member.Member is PropertyInfo propertyInfo))
            {
                throw new InvalidOperationException("Member expression member isn't a property info. There is nothing to return.");
            }

            return propertyInfo;
        }
    }
}
