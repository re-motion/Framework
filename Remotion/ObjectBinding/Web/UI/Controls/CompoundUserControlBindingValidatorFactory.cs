using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [ImplementationFor (typeof (IUserControlBindingValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundUserControlBindingValidatorFactory : CompoundValidatorFactory<UserControlBinding>, IUserControlBindingValidatorFactory
  {
    public CompoundUserControlBindingValidatorFactory (IEnumerable<IUserControlBindingValidatorFactory> innerFactories)
        : base(innerFactories)
    {
    }
  }
}