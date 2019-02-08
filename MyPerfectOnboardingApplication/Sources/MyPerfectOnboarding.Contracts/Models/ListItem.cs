using System;
using System.Linq.Expressions;
using System.Reflection;
using MyPerfectOnboarding.Contracts.Selectors;

namespace MyPerfectOnboarding.Contracts.Models
{
    public class ListItem : IListItem
    {
        internal ListItem() { }

        public ListItem(IListItem item) 
            : this()
        {
            Id = item.Id;
            Text = item.Text;
            IsActive = item.IsActive;
            CreationTime = item.CreationTime;
            LastUpdateTime = item.LastUpdateTime;
        }

        private ListItem(ListItem originalItem, PropertyInfo propertyForNewValue, object newValue)
            :this()
        {
            var valueSelector = ValueSelector.Create(originalItem, propertyForNewValue, newValue);

            Id = valueSelector.For(item => item.Id);
            Text = valueSelector.For(item => item.Text);
            IsActive = valueSelector.For(item => item.IsActive);
            CreationTime = valueSelector.For(item => item.CreationTime);
            LastUpdateTime = valueSelector.For(item => item.LastUpdateTime);
        }

        public Guid Id { get; }

        public string Text { get; }

        public bool IsActive { get; }

        public DateTime CreationTime { get; }

        public DateTime LastUpdateTime { get; }

        public override string ToString() 
            => $"{nameof(Id)}: {Id}, {nameof(Text)}: {Text}, {nameof(IsActive)}: {IsActive}, {nameof(CreationTime)}: {CreationTime}, {nameof(LastUpdateTime)}: {LastUpdateTime}";

        public ListItem With<TValue>(Expression<Func<IListItem, TValue>> propertySelector, TValue newPropertyValue)
        {
            var propertyInfo = propertySelector.ToPropertyInfo();

            var clone = new ListItem(this, propertyInfo, newPropertyValue);

            return clone;
        }
    }
}
