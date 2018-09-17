using MyPerfectOnboarding.Contracts;
using System;

namespace MyPerfectOnboarding.Api.Services
{
    internal class UrlLocation : IUrlLocation
    {
        public string GetLocation( Guid id)
        {
            return "aaa" + id;
        }
    }
}
