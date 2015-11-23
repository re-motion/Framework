using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocBooleanValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocBooleanValueValidatorFactory : IBocBooleanValueValidatorFactory
  {
    public const int Position = 0;

    private const string c_nullString = "null";

    public IEnumerable<BaseValidator> CreateValidators (IBocBooleanValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly || !control.IsRequired)
        yield break;

      var resourceManager = control.GetResourceManager();
      yield return CreateRequiredFieldValidator (control, resourceManager);
    }


    private CompareValidator CreateRequiredFieldValidator (IBocBooleanValue control, IResourceManager resourceManager)
    {
      var notNullItemValidator = new CompareValidator ();
      notNullItemValidator.ID = control.ID + "_ValidatorNotNullItem";
      notNullItemValidator.ControlToValidate = control.ID;
      notNullItemValidator.ValueToCompare = c_nullString;
      notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
      notNullItemValidator.ErrorMessage = resourceManager.GetString (BocBooleanValue.ResourceIdentifier.NullItemValidationMessage);
      
      return notNullItemValidator;
    }
  }
}