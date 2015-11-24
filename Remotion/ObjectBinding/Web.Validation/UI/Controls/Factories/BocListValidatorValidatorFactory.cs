using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories
{
  [ImplementationFor (typeof (IBocListValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocListValidatorValidatorFactory : IBocListValidatorFactory
  {
    public const int Position = BocListValidatorFactory.Position + 1;
    public IEnumerable<BaseValidator> CreateValidators (IBocList control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      yield return CreateBocListValidator (control);
    }

    private BaseValidator CreateBocListValidator (IBocList control)
    {
      var bocValidator = new BocListValidator();
      bocValidator.ControlToValidate = control.ID;
      bocValidator.ID = control.ID + "_BocListValidator";
      return bocValidator;
    }
  }
}