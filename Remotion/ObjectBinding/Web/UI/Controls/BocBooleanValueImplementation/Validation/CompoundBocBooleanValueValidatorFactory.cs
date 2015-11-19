using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocBooleanValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  internal class CompoundBocBooleanValueValidatorFactory : CompoundValidatorFactory<IBocBooleanValue>, IBocBooleanValueValidatorFactory
  {
    public CompoundBocBooleanValueValidatorFactory (IEnumerable<IBocBooleanValueValidatorFactory> innerFactories)
        : base (innerFactories)
    { }
  }
}