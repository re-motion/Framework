using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocReferenceValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  internal class CompoundBocReferenceValueValidatorFactory : CompoundValidatorFactory<IBocReferenceValue>, IBocReferenceValueValidatorFactory
  {
    public CompoundBocReferenceValueValidatorFactory (IEnumerable<IBocReferenceValueValidatorFactory> innerFactories)
        : base (innerFactories)
    { }
  }
}