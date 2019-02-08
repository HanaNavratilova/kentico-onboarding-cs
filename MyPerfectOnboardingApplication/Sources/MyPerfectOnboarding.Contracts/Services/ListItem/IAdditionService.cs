using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IAdditionService
    {
        Task<IListItem> AddItemAsync(Models.ListItem item);
    }
}
