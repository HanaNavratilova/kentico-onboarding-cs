using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Hosting;
using System.Web.Http.Metadata;
using System.Web.Http.Validation;

namespace MyPerfectOnboarding.Dependency.DependencyResolvers
{
    internal class DependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        private static readonly List<string> UnityExceptions = new List<string>
        {
            nameof(IHostBufferPolicySelector),
            nameof(IHttpControllerSelector),
            nameof(IHttpControllerActivator),
            nameof(IHttpActionSelector),
            nameof(IHttpActionInvoker),
            nameof(IContentNegotiator),
            nameof(IExceptionHandler),
            nameof(ModelMetadataProvider),
            nameof(IModelValidatorCache)
        };

        public DependencyResolver(IUnityContainer container)
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
            catch (ResolutionFailedException exception)
                when (UnityExceptions.Contains(exception.TypeRequested))
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
            catch (ResolutionFailedException exception)
                when (UnityExceptions.Contains(exception.TypeRequested))
            {
                return Enumerable.Empty<object>();
            }
        }
    }
}