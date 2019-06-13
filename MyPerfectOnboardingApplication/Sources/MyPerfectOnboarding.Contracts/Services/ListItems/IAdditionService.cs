using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItems
{
    public interface IAdditionService
    {
        Task<ListItem> AddItemAsync(ListItem item);
    }
}
