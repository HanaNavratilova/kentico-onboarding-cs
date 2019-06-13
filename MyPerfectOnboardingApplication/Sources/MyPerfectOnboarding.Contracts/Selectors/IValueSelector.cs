using System;
using System.Linq.Expressions;

namespace MyPerfectOnboarding.Contracts.Selectors
{
    public interface IValueSelector<TModel>
    {
        TValue For<TValue>(Expression<Func<TModel, TValue>> valuePropertySelector);
    }
}
