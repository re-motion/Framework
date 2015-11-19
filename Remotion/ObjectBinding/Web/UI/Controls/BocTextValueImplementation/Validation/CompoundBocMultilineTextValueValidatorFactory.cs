using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocMultilineTextValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundBocMultilineTextValueValidatorFactory : CompoundValidatorFactory<IBocMultilineTextValue>, IBocMultilineTextValueValidatorFactory
  {
    public CompoundBocMultilineTextValueValidatorFactory (IEnumerable<IBocMultilineTextValueValidatorFactory> innerFactories)
        : base(innerFactories)
    {
    }
  }
}