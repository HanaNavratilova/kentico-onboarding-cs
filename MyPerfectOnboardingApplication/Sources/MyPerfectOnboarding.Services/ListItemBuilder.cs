using System;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Services
{
    internal class ListItemBuilder
    {
        private IListItem _item;

        internal ListItemBuilder() { }

        internal IListItem BuildItem(ListItem listItem)
        {
            _item = new Item(listItem);

            return _item;
        }
    }

    internal class Item : IListItem
    {
        internal Item(ListItem item)
        {
            Id = item.Id;
            Text = item.Text;
            IsActive = item.IsActive;
            CreationTime = item.CreationTime;
            LastUpdateTime = item.LastUpdateTime;
        }

        public Guid Id { get; }

        public string Text { get; }

        public bool IsActive { get; }

        public DateTime CreationTime { get; }

        public DateTime LastUpdateTime { get; }
    }
}
