using System;
using System.Collections.Generic;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [ImplementationFor (typeof (IBocAutoCompleteReferenceValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  internal class CompoundBocAutoCompleteReferenceValueValidatorFactory : CompoundValidatorFactory<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValueValidatorFactory
  {
    public CompoundBocAutoCompleteReferenceValueValidatorFactory (IEnumerable<IBocAutoCompleteReferenceValueValidatorFactory> innerFactories)
        : base (innerFactories)
    { }
  }
}