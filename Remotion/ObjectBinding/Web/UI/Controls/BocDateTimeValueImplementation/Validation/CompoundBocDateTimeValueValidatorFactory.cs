using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocDateTimeValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  internal class CompoundBocDateTimeValueValidatorFactory : CompoundValidatorFactory<IBocDateTimeValue>, IBocDateTimeValueValidatorFactory
  {
    public CompoundBocDateTimeValueValidatorFactory (IEnumerable<IBocDateTimeValueValidatorFactory> innerFactories)
        : base (innerFactories)
    { }
  }
}