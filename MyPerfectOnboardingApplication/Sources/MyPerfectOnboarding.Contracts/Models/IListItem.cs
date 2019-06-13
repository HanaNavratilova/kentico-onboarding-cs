using System;

namespace MyPerfectOnboarding.Contracts.Models
{
    public interface IListItem
    {
        Guid Id { get; }

        string Text { get; }

        bool IsActive { get; }

        DateTime CreationTime { get; }

        DateTime LastUpdateTime { get; }
    }
}
