using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocCheckBoxValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundBocCheckBoxValidatorFactory : CompoundValidatorFactory<IBocCheckBox>, IBocCheckBoxValidatorFactory
  {
    public CompoundBocCheckBoxValidatorFactory (IEnumerable<IBocCheckBoxValidatorFactory> innerFactories)
        : base(innerFactories)
    {
    }
  }
}