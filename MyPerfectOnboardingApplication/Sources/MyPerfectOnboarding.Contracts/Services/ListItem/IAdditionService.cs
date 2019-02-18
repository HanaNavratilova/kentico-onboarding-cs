using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IAdditionService
    {
        Task<Models.ListItem> AddItemAsync(Models.ListItem item);
    }
}
