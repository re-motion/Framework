using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories
{

  [ImplementationFor (typeof (IUserControlBindingValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class UserControlBindingValidatorValidatorFactory : IUserControlBindingValidatorFactory
  {
    public const int Position = 1;

    public IEnumerable<BaseValidator> CreateValidators (UserControlBinding control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      yield return CreateBocListValidator (control);
    }

    private BaseValidator CreateBocListValidator (UserControlBinding control)
    {
      var bocValidator = new UserControlBindingValidator();
      bocValidator.ControlToValidate = control.ID;
      bocValidator.ID = control.ID + "_BocListValidator";
      return bocValidator;
    }
  }
}