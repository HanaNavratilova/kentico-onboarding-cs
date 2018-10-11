using System;

namespace MyPerfectOnboarding.Contracts.Services.Database.Generators
{
    public interface ITimeGenerator
    {
        DateTime GetCurrentTime();
    }
}
