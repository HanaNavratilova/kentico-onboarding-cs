using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IEditingService
    {
        Task ReplaceItemAsync(Guid id, IListItem editedItem);
    }
}
