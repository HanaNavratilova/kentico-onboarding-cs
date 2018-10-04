using System;
using NUnit.Framework.Constraints;

namespace MyPerfectOnboarding.Api.Tests.Utils
{
    public static class EqualConstraintExtension
    {
        private static readonly Lazy<ListItemEqualityComparer> LazyComparer = new Lazy<ListItemEqualityComparer>();

        public static EqualConstraint UsingComparer(this EqualConstraint equalConstraint)
            => equalConstraint.Using(LazyComparer.Value);
    }
}
