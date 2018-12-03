using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IPutService
    {
        Task ReplaceItemAsync(Models.ListItem editedItem);
    }
}
