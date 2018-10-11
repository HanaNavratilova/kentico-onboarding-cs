using System;
using MyPerfectOnboarding.Contracts.Services.Database.Generators;

namespace MyPerfectOnboarding.Services.Generators
{
    internal class GuidGenerator : IGuidGenerator
    {
        public Guid Generate()
            => Guid.NewGuid();
    }
}
