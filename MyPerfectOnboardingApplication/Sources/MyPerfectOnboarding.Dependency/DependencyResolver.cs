using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using MyPerfectOnboarding.Contracts.Dependency;
using Unity;
using Unity.Exceptions;

namespace MyPerfectOnboarding.Dependency
{
    internal class DependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public DependencyResolver(IContainer container)
        {
            _container = container;
        }

        public IDependencyScope BeginScope()
        {
            var child = _container.CreateChildContainer();
            return new DependencyResolver(child);
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return Enumerable.Empty<object>();
            }
        }
    }
}