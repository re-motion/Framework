using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  [ImplementationFor(typeof(IBocListValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  internal class CompoundBocListValidatorFactory : CompoundValidatorFactory<IBocList>, IBocListValidatorFactory
  {
    public CompoundBocListValidatorFactory (IEnumerable<IBocListValidatorFactory> innerFactories)
        : base(innerFactories)
    {
    }
  }
}