using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Dependencies;

namespace MyPerfectOnboarding.Contracts.Dependency
{
    public interface IContainer : IDisposable
    {
       IContainer RegisterBootstraper<TBootstraper>() where TBootstraper: IBootstraper, new();

        IContainer RegisterType<TIType, TType>() where TType : TIType;

        IContainer RegisterType<TType>(Func<TType> function);

       IContainer CreateChildContainer();

        object Resolve(Type serviceType);

        IEnumerable<object> ResolveAll(Type serviceType);
    }
}
