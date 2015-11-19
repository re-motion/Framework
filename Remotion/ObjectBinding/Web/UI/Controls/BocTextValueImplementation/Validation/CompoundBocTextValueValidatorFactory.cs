using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation
{
  [ImplementationFor(typeof(IBocTextValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  internal class CompoundBocTextValueValidatorFactory:CompoundValidatorFactory<IBocTextValue>, IBocTextValueValidatorFactory
  {
    public CompoundBocTextValueValidatorFactory (IEnumerable<IBocTextValueValidatorFactory> innerFactories)
        : base(innerFactories)
    {
    }
  }
}