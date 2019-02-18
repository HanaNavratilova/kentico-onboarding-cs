using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Services.ListItems
{
    public interface IAdditionService
    {
        Task<ListItem> AddItemAsync(ListItem item);
    }
}
