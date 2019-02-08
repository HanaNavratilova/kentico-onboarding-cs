using System;
using System.Linq.Expressions;

namespace MyPerfectOnboarding.Contracts.Selector
{
    public interface IValueSelector<TModel>
    {
        TValue For<TValue>(Expression<Func<TModel, TValue>> valuePropertySelector);
    }
}
