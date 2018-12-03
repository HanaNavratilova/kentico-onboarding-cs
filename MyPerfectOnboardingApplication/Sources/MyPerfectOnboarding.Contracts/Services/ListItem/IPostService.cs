using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IPostService
    {
        Task<Models.ListItem> AddItemAsync(Models.ListItem item);
    }
}
