using System;
using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IEditingService
    {
        Task ReplaceItemAsync(Guid id, Models.ListItem editedItem);
    }
}
