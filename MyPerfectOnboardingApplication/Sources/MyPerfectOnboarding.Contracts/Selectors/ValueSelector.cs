using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MyPerfectOnboarding.Contracts.Selectors
{
    internal static class ValueSelector
    {
        internal static IValueSelector<TModel> Create<TModel>(TModel originalItem, PropertyInfo propertyForNewValue, object newValue)
            => new ValueSelectorImplementation<TModel>(originalItem, propertyForNewValue, newValue);

        private class ValueSelectorImplementation<TModel> : IValueSelector<TModel>
        {
            private readonly TModel _originalItem;
            private readonly PropertyInfo _propertyForNewValue;
            private readonly object _newValue;

            internal ValueSelectorImplementation(TModel originalItem, PropertyInfo propertyForNewValue, object newValue)
            {
                _originalItem = originalItem;
                _propertyForNewValue = propertyForNewValue;
                _newValue = newValue;
            }

            public TValue For<TValue>(Expression<Func<TModel, TValue>> valuePropertySelector)
            {
                var propertyInfo = valuePropertySelector.ToPropertyInfo();

                return (TValue)(_propertyForNewValue.Name == propertyInfo.Name
                    ? _newValue
                    : propertyInfo.GetValue(_originalItem));
            }
        }
    }
}
