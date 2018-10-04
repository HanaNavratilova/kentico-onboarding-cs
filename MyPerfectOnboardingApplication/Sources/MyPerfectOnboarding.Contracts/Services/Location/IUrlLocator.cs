using System;

namespace MyPerfectOnboarding.Contracts.Services.Location
{
    public interface IUrlLocator
    {
        Uri GetListItemLocation(Guid id);
    }
}
