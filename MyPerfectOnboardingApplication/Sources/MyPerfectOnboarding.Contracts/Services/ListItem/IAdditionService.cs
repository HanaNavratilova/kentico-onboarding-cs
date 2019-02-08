using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IAdditionService
    {
        Task<Models.ListItem> AddItemAsync(Models.ListItem item);
    }
}
