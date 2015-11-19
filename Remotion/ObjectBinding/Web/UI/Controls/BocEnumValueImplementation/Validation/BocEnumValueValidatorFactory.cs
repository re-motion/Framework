using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocEnumValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class BocEnumValueValidatorFactory : IBocEnumValueValidatorFactory
  {
    public IEnumerable<BaseValidator> CreateValidators (IBocEnumValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly || !control.IsRequired)
        yield break;

      var resourceManager = control.GetResourceManager();
      yield return CreateRequiredFieldValidator (control, resourceManager);
    }

    private RequiredFieldValidator CreateRequiredFieldValidator (IBocEnumValue control, IResourceManager resourceManager)
    {
      var requiredValidator = new RequiredFieldValidator ();
      requiredValidator.ID = control.ID + "_ValidatorRequried";
      requiredValidator.ControlToValidate = control.TargetControl.ID;
      requiredValidator.ErrorMessage = resourceManager.GetString (BocBooleanValue.ResourceIdentifier.NullItemValidationMessage);
      return requiredValidator;
    }
  }
}