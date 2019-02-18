using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItems
{
    public interface IEditingService
    {
        Task<ListItem> ReplaceItemAsync(Guid id, ListItem editedItem);
    }
}
