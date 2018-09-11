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
    }
}
