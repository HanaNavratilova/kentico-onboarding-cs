using System;
using MyPerfectOnboarding.Contracts.Services.Database.Generators;

namespace MyPerfectOnboarding.Services.Generators
{
    internal class TimeGenerator : ITimeGenerator
    {
        public DateTime GetCurrentTime()
            => DateTime.Now;
    }
}
