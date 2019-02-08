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
            //It's needed because of the tests. They failed cause ctor was called before every test, but the serializer were already registered.
            BsonSerializer.RegisterSerializer(new ImpliedImplementationInterfaceSerializer<IListItem, ListItemModel>());
        }

        public IContainer RegisterTypesTo(IContainer container)
            => container
                .Register<IListRepository, ListRepository>(Lifetime.PerRequest);
    }
}
