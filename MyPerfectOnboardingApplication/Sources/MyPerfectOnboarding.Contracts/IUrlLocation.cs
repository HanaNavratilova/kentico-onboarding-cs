using System;

namespace MyPerfectOnboarding.Contracts
{
    public interface IUrlLocation
    {
        string GetLocation(Guid id);
    }
}
