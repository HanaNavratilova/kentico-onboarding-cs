using MyPerfectOnboarding.Tests.Utils.Comparers;
using NUnit.Framework.Constraints;

namespace MyPerfectOnboarding.Tests.Utils.Extensions
{
    public static class EqualConstraintExtensions
    {
        public static EqualConstraint UsingListItemComparer(this EqualConstraint equalConstraint) 
            => equalConstraint.Using(ListItemEqualityComparer.Instance);

        public static CollectionItemsEqualConstraint UsingListItemComparer(this CollectionEquivalentConstraint equalConstraint)
            => equalConstraint.Using(ListItemEqualityComparer.Instance);
    }
}
