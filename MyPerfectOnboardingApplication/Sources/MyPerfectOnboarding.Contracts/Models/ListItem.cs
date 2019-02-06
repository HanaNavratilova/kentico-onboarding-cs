using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MyPerfectOnboarding.Contracts.Models
{
    public class ListItem
    {
        public ListItem() { }

        public ListItem(IListItem item)
        {
            Id = item.Id;
            Text = item.Text;
            IsActive = item.IsActive;
            CreationTime = item.CreationTime;
            LastUpdateTime = item.LastUpdateTime;
        }

        public Guid Id { get; private set; }

        public string Text { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreationTime { get; private set; }

        public DateTime LastUpdateTime { get; private set; }

        public override string ToString() 
            => $"{nameof(Id)}: {Id}, {nameof(Text)}: {Text}, {nameof(IsActive)}: {IsActive}, {nameof(CreationTime)}: {CreationTime}, {nameof(LastUpdateTime)}: {LastUpdateTime}";

        public ListItem With<T>(Expression<Func<ListItem, T>> function, T parameter)
        {
            var member = (MemberExpression) function.Body;
            var propertyInfo = (PropertyInfo)(member).Member;
            propertyInfo.SetValue(this, parameter);

            return this;
        }
    }
}
