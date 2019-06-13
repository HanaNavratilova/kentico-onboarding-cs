using System;
using MyPerfectOnboarding.Contracts.Services.Generators;

namespace MyPerfectOnboarding.Services.Generators
{
    internal class TimeGenerator : ITimeGenerator
    {
        public DateTime GetCurrentTime()
            => DateTime.UtcNow;
    }
}
