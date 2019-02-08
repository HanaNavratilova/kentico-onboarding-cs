using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Database.Models;
using MyPerfectOnboarding.Database.Repository;

namespace MyPerfectOnboarding.Database
{
    public class DatabaseBootstraper : IBootstraper
    {
        static DatabaseBootstraper()
        {
            // Static constructor is needed because BsonSerilizer can only be set once in application's lifetime
            // (and nothing prevented this class from being re-instantiated).
            BsonSerializer.RegisterSerializer(new ImpliedImplementationInterfaceSerializer<IListItem, ListItemModel>());
        }

        public IContainer RegisterTypesTo(IContainer container)
            => container
                .Register<IListRepository, ListRepository>(Lifetime.PerRequest);
    }
}
