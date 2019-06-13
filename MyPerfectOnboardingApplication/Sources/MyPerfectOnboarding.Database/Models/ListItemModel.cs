using System;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Database.Models
{
    internal class ListItemModel : IListItem
    {
        public ListItemModel() { }

        public ListItemModel(IListItem item)
        {
            Id = item.Id;
            Text = item.Text;
            IsActive = item.IsActive;
            CreationTime = item.CreationTime;
            LastUpdateTime = item.LastUpdateTime;
        }

        public Guid Id { get; set; }

        public string Text { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public override string ToString()
            => $"{nameof(Id)}: {Id}, {nameof(Text)}: {Text}, {nameof(IsActive)}: {IsActive}, {nameof(CreationTime)}: {CreationTime}, {nameof(LastUpdateTime)}: {LastUpdateTime}";
    }
}
