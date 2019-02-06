using System;
using System.Collections.Generic;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Tests.Utils.Comparers
{
    public sealed class ListItemEqualityComparer : IEqualityComparer<IListItem>
    {
        private static readonly Lazy<ListItemEqualityComparer> LazyComparer = new Lazy<ListItemEqualityComparer>(() => new ListItemEqualityComparer());

        public static IEqualityComparer<IListItem> Instance => LazyComparer.Value;

        private ListItemEqualityComparer() { }

        public bool Equals(IListItem x, IListItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            //if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id) && string.Equals(x.Text, y.Text) && x.IsActive == y.IsActive && x.CreationTime.Equals(y.CreationTime) && x.LastUpdateTime.Equals(y.LastUpdateTime);
        }

        public int GetHashCode(IListItem obj)
        {
            unchecked
            {
                var hashCode = obj.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Text != null ? obj.Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.IsActive.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.CreationTime.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.LastUpdateTime.GetHashCode();
                return hashCode;
            }
        }
    }
}
