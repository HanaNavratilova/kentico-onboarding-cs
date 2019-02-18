using MyPerfectOnboarding.Tests.Utils.Comparers;
using NUnit.Framework.Constraints;

namespace MyPerfectOnboarding.Tests.Utils.Extensions
{
    public static class EqualConstraintExtension
    {
        public static EqualConstraint UsingComparer(this EqualConstraint equalConstraint)
            => equalConstraint.Using(ListItemEqualityComparer.Instance);
    }
}
