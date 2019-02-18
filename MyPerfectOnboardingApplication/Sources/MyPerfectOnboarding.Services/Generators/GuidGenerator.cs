using System;
using MyPerfectOnboarding.Contracts.Services.Generators;

namespace MyPerfectOnboarding.Services.Generators
{
    internal class GuidGenerator : IGuidGenerator
    {
        public Guid Generate()
            => Guid.NewGuid();
    }
}
