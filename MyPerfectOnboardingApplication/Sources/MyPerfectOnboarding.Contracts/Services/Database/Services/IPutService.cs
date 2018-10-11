using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.Database.Services
{
    public interface IPutService
    {
        Task ReplaceItemAsync(ListItem editedItem);
    }
}
