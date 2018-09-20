using System;

namespace MyPerfectOnboarding.Contracts.Models
{
    public class ListItem
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Text)}: {Text}, {nameof(IsActive)}: {IsActive}, {nameof(CreationTime)}: {CreationTime}, {nameof(LastUpdateTime)}: {LastUpdateTime}";
        }
    }
}
