using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IEditingService
    {
        Task ReplaceItemAsync(Models.ListItem editedItem);
    }
}
