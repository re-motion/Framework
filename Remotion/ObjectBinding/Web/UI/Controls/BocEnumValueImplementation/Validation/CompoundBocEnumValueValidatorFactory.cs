using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocEnumValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  internal class CompoundBocEnumValueValidatorFactory : CompoundValidatorFactory<IBocEnumValue>, IBocEnumValueValidatorFactory
  {
    public CompoundBocEnumValueValidatorFactory (IEnumerable<IBocEnumValueValidatorFactory> innerFactories)
        : base (innerFactories)
    { }
  }
}