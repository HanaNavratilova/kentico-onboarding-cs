using System;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Api.Models
{
    public class ListItemViewModel : IListItem
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public override string ToString()
            => $"{nameof(Id)}: {Id}, {nameof(Text)}: {Text}, {nameof(IsActive)}: {IsActive}, {nameof(CreationTime)}: {CreationTime}, {nameof(LastUpdateTime)}: {LastUpdateTime}";
    }
}